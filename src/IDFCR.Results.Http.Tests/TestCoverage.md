# IDFCR.Results.Http Test Coverage

## Overview
The test suite for IDFCR.Results.Http validates the core functionality of converting `IUnitResult` objects into HTTP responses suitable for API endpoints.

## What This Project Does
The IDFCR.Results.Http library provides:
1. **HTTP Status Code Mapping**: Automatically maps `FailureReason` enum values to appropriate HTTP status codes
2. **JSON Serialization**: Converts unit results into standardized JSON responses with metadata
3. **Content Negotiation**: Validates Accept headers to ensure only JSON responses are returned
4. **Extension Methods**: Simple `.AsHttp()` extension to convert any `IUnitResult` into an `IUnitHttpResult`

## Test Categories

### 1. Status Code Mapping Tests (10 tests)
These tests verify that each `FailureReason` maps to the correct HTTP status code:

- **NotFound** → 404 Not Found
- **ValidationError** → 400 Bad Request  
- **Conflict** → 409 Conflict
- **Unauthorized** → 401 Unauthorized
- **Forbidden** → 403 Forbidden
- **InternalError** → 500 Internal Server Error
- **ExternalDependencyError** → 424 Failed Dependency
- **AuthorizationError** → 401 Unauthorized
- **Unknown** → 503 Service Unavailable
- **Success (None)** → 200 OK

Each test:
- Creates a failed result with a specific `FailureReason`
- Executes it through the HTTP context
- Verifies both `_httpResponse.StatusCode` and `result.GetStatusCode()` return the expected value

### 2. JSON Serialization Tests (5 tests)
These tests validate the JSON structure and content:

#### ExecuteAsync_WithSuccessResult_WritesCorrectJsonStructure
- Verifies successful results include `is_success` (true) and `_meta` properties
- Confirms custom metadata is included in the `_meta` object
- Validates the `action` is correctly serialized

#### ExecuteAsync_WithFailureResult_IncludesFailureReasonInMeta
- Verifies failed results include `is_success` (false) 
- Confirms `failure_reason` appears in `_meta` with the correct enum name
- Validates custom metadata is preserved

#### ExecuteAsync_WithTypedResult_IncludesValueInResponse
- Tests `IUnitResult<T>` serialization behavior
- Verifies the response structure is valid JSON
- Confirms metadata and success status are present

#### ExecuteAsync_WithResultCollection_SerializesSuccessfully
- Tests `IUnitResultCollection<T>` serialization
- Validates collection results produce valid JSON responses
- Confirms metadata is correctly included

#### ExecuteAsync_WithMultipleMetadata_IncludesAllInResponse
- Verifies multiple metadata key-value pairs are all included
- Tests that metadata chaining works correctly

### 3. Content Negotiation Tests (5 tests)
These tests ensure proper HTTP content negotiation:

#### ExecuteAsync_WithValidJsonAcceptHeader_ReturnsJsonResponse
- Confirms `Accept: application/json` is properly handled
- Validates response is parseable JSON

#### ExecuteAsync_WithEmptyAcceptHeader_AcceptsRequest  
- Tests that empty Accept header defaults to allowing JSON

#### ExecuteAsync_WithMissingAcceptHeader_AcceptsRequest
- Tests that missing Accept header defaults to allowing JSON  

#### ExecuteAsync_WithXmlAcceptHeader_Returns406
- Verifies non-JSON Accept headers return 406 Not Acceptable
- Confirms error message is "Invalid accept header"

#### ExecuteAsync_WithHtmlAcceptHeader_Returns406
- Additional validation for unsupported content types

### 4. UnitAction Tests (1 parameterized test = 4 test cases)
#### ExecuteAsync_WithDifferentActions_IncludesCorrectActionInMeta
- Parameterized test covering all `UnitAction` enum values:
  - Add
  - Update
  - Delete  
  - Get
- Verifies the action appears correctly in JSON metadata

### 5. Edge Cases (3 tests)
#### ExecuteAsync_WithNullValueTypedResult_DoesNotIncludeValueProperties
- Tests behavior when `IUnitResult<T>` has no value
- Verifies response still includes success status and metadata

#### ExecuteAsync_WithEmptyCollection_SerializesSuccessfully  
- Tests `IUnitResultCollection<T>` with empty array
- Confirms valid JSON response is generated

## Key Improvements Over Previous Tests

### What Was Wrong Before
1. **Limited Status Code Coverage**: Only tested success (200) and one failure scenario
2. **No FailureReason Validation**: Didn't verify the mapping of all failure reasons to HTTP status codes
3. **Incomplete Content Negotiation**: Only tested XML rejection, not edge cases like empty headers
4. **Weak Metadata Testing**: Didn't thoroughly test multiple metadata values or failure scenarios
5. **Missing Edge Cases**: No tests for null values, empty collections, or different actions
6. **Poor Test Names**: Generic names like "ExecuteAsyncWithResultPayload" don't describe what they test

### What's Better Now
1. **Comprehensive Status Code Testing**: Every `FailureReason` is tested
2. **Clear Test Organization**: Tests are grouped into logical categories with regions
3. **Better Test Names**: Names clearly describe what's being tested (e.g., `GetStatusCode_WithValidationError_Returns400`)
4. **Edge Case Coverage**: Tests for null values, empty collections, multiple metadata
5. **Content Negotiation Validation**: Tests all scenarios: valid JSON, empty header, missing header, unsupported types
6. **Correct Assertions**: Uses proper property names (`is_success` not `isSuccess`, `failure_reason` not `failureReason`)
7. **Action Coverage**: Parameterized tests for all UnitAction values

## JSON Response Structure

All HTTP results follow this standardized structure:

```json
{
  "is_success": true/false,
  "_meta": {
    "action": "Add|Update|Delete|Get|...",
    "failure_reason": "ValidationError|NotFound|..." // only present on failures
    // ... additional custom metadata
  }
}
```

## Running the Tests

Run all tests:
```bash
dotnet test src\IDFCR.Results.Http.Tests\IDFCR.Results.Http.Tests.csproj
```

Run specific category:
```bash
# Status code tests
dotnet test --filter "FullyQualifiedName~GetStatusCode"

# JSON serialization tests  
dotnet test --filter "FullyQualifiedName~ExecuteAsync"

# Content negotiation
dotnet test --filter "FullyQualifiedName~AcceptHeader"
```

## Test Metrics
- **Total Tests**: 26 (20 unique + 4 parameterized + 2 edge cases)
- **Code Coverage**: Core functionality of status code mapping, JSON serialization, and content negotiation
- **Execution Time**: ~237ms for full suite
