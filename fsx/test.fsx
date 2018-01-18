open System.Diagnostics

let p = new Process()
p.StartInfo.FileName <- "cmd.exe"
//p.StartInfo.Arguments = @"/c D:\\pdf2xml";
p.StartInfo.UseShellExecute <- false;
p.StartInfo.RedirectStandardOutput <- true;
p.StartInfo.RedirectStandardInput <- true;
p.Start()

p.StandardInput.WriteLine(@"cd C:\r\Platform");
