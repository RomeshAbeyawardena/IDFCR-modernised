using IDFCR.Abstractions.Cli.ManagedStreams;
using System;
using System.Collections.Generic;
using System.Text;

namespace IDFCR.TestUtilities
{
    internal class StringWriteableStream(StringWriter writer) : WriteableStreamBase(writer)
    {

    }
}
