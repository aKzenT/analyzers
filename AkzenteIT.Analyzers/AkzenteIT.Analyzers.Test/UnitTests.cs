using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;

namespace AkzenteIT.Analyzers.Test
{
    [TestClass]
    public class UnitTest : DiagnosticVerifier
    {

        //No diagnostics expected to show up
        [TestMethod]
        public void EmptyCodeDoesNotRaiseError()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }
        
        [TestMethod]
        public void UnawaitedTaskRaisesError()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TypeName
        {   
            private void method()
            {
                Task.Delay(200);
            }
        }
    }";
            var expected = new DiagnosticResult
            {
                Id = "AkzenteITAnalyzers",
                Message = "The expression resulted in a task object that was not awaited or otherwise used.",
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 17)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }
        

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new TaskNotUsedAnalyzer();
        }
    }
}