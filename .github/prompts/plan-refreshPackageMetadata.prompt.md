## Plan: Refresh Package Metadata

Update `dbo.GetNextPackageVersion` so that when the procedure resolves an existing `PackageId` from `dbo.Package`, it also reconciles `Alias` and `Description` against the incoming values before calculating the next revision. This keeps package metadata current without changing the procedure contract, lookup key, or version/tag behavior.

**Steps**
1. Confirm the existing lookup contract remains unchanged: continue resolving the package row by trimmed `@packageName`, assign the matching row's `PackageId` to the existing local `@packageId` variable, and treat a non-null looked-up value as the signal that this is the existing-package path rather than a new package insert. Do not add `@packageId` as an input parameter or otherwise change the procedure signature.
2. In the existing-package branch of `c:\Users\romes\OneDrive\dev\IDFCR_main\initial.storedproc.sql`, add a targeted `UPDATE dbo.Package` that runs before the `PackageVersion` revision query. Require a null-safe conditional update so the row is only updated when `Alias` or `Description` actually differs from the incoming values. The implementation should express that difference check explicitly with `IS NULL` and `IS NOT NULL` logic rather than relying on a bare `<>` comparison. A direct `UPDATE ... WHERE PackageId = @packageId AND (...)` pattern is preferred so the comparison is evaluated at update time inside the transaction.
3. Keep the current transaction and locking model intact by reusing the row already read under `UPDLOCK, HOLDLOCK`, and avoid broadening the scope into tag or package-name reconciliation. This step depends on step 2.
4. Preserve existing duplicate-key error behavior instead of adding custom alias-precheck logic. Let the `UQ_Package_ALIAS` constraint continue to raise 2627/2601 if a requested alias is already assigned to another package. This depends on step 2.
5. Verify that the revision-number logic remains unchanged: existing packages still compute `ISNULL(MAX(RevisionNumber), -1) + 1`, new packages still insert with revision 0, and metadata refresh does not alter version sequencing. This depends on steps 2-4.
6. Run focused SQL validation cases after the change:
   a. existing package with changed alias only
   b. existing package with changed description only
   c. existing package with both values unchanged
   d. existing package with alias collision against another package
   e. existing package with `Alias` changing from `NULL` to a non-null value
   f. existing package with `Alias` changing from a non-null value to `NULL`
   g. existing package with `Description` changing from `NULL` to a non-null value, and from a non-null value to `NULL`
   h. existing package with both incoming values `NULL` and both stored values already `NULL`
   i. two concurrent executions attempting to assign the same alias to different packages
   j. new package insert path

**Relevant files**
- `c:\Users\romes\OneDrive\dev\IDFCR_main\initial.storedproc.sql` — modify `dbo.GetNextPackageVersion`, specifically the existing-package branch after package lookup and before revision calculation.
- `c:\Users\romes\OneDrive\dev\IDFCR_main\initial.sql` — reference `dbo.Package` schema and the `UQ_Package_ALIAS` uniqueness constraint to preserve expected error semantics.
- `c:\Users\romes\OneDrive\dev\IDFCR_main\GetNextPackage.sql` — use as a manual execution template for validating the procedure after the change.
- `c:\Users\romes\OneDrive\dev\IDFCR_main\get-next-package-version.ps1` — optional execution path if validation is normally driven through PowerShell instead of direct SQL.

**Verification**
1. Seed one package row, then execute the procedure with the same `@packageName` and a different alias; confirm `dbo.Package.Alias` changes and a new `PackageVersion` row is created.
2. Repeat with only `@packageDescription` changed; confirm description refreshes without affecting name or prior versions.
3. Execute again with identical alias/description inputs, including a case where both stored values and both inputs are `NULL`; confirm no duplicate package row is created and version increment behavior is unchanged.
4. Verify `Alias` and `Description` null transitions explicitly: `NULL` to non-null and non-null to `NULL` should both update the package metadata correctly and still create the next `PackageVersion` row.
5. Seed a second package using the target alias, then call the procedure for the first package with that alias; confirm the procedure still returns the existing duplicate-key error path.
6. Run two concurrent executions that attempt to assign the same alias to different packages; confirm one succeeds and the other fails through the existing duplicate-key path.
7. Execute the new-package path and confirm insert behavior for `Package`, `PackageVersion`, and version tags is unchanged.

**Decisions**
- Included: refreshing `Alias` and `Description` for an existing package record found by current name-based lookup.
- Excluded: changing the procedure signature, adding `@packageId` as an input, reconciling `PackageTag`, trimming/normalizing alias or description beyond current behavior, or introducing audit columns.
- Assumption: the user’s “find an existing package ID” refers to the current code path where the procedure looks up `PackageId` by package name and then should update metadata on that located row.
- Input-shape decision: preserve `@packageAlias` and `@packageDescription` exactly as provided by current callers; this change adds metadata refresh only and does not introduce new trimming, normalization, or empty-string-to-`NULL` conversion semantics.

**Further Considerations**
1. If whitespace normalization for alias or description becomes necessary, treat that as a separate behavioral change with its own validation because it would alter persisted values rather than only refreshing stale metadata.
2. If the generic duplicate-key error message becomes too coarse once alias-update collisions are possible, consider a follow-up change to improve error specificity without changing transaction semantics.
3. If package metadata drift matters operationally, consider a future schema change for `ModifiedTimestampUtc`; recommendation: defer, because it is outside the requested procedure fix.
