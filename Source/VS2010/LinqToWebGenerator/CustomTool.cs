using System;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;

using linqtoweb.CodeGenerator;
using linqtoweb.CodeGenerator.AST;

namespace LinqToWebGenerator
{
    [Guid("55CA59B6-C689-4FAC-B494-5368229E7E2B")]
    [ComVisible(true)]
    public class CustomTool : Microsoft.VisualStudio.TextTemplating.VSHost.BaseCodeGeneratorWithSite
    {
        public override string GetDefaultExtension()
        {
            return ".design.cs";
        }

        protected override byte[] GenerateCode(string inputFileName, string inputFileContent)
        {
            string code = string.Empty;

            Scanner scanner = new Scanner();
            scanner.SetSource(inputFileContent, 0);

            try
            {

                Parser parser = new Parser(scanner);
                if (parser.Parse())
                {
                    GlobalCode x = parser.Ast;

                    var ms = new MemoryStream();
                    var sw = new StreamWriter(ms, Encoding.Unicode);

                        x.EmitCs(sw, FileNamespace, Path.GetFileNameWithoutExtension(inputFileName));

                        ms.Position = 0;
                        code = new StreamReader(ms).ReadToEnd();
                    
                }
                else
                {
                    throw new Exception("Syntax error.");
                }

            }
            catch (GeneratorException e)
            {
                this.GeneratorErrorCallback(false, 0, e.Message, e.Position.StartLine, e.Position.StartColumn);
            }
            catch (Exception e)
            {
                this.GeneratorErrorCallback(false, 0, e.Message, 0, 0);
            }

            return Encoding.UTF8.GetBytes(code);
        }
    }

}
