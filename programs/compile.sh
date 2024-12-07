filename=$(basename "$1" .fors)

dotnet run --no-build --project ../ForsMachine.Compiler/ForsMachine.Compiler.csproj -- $filename.fors > $filename.asm && customasm $filename.asm
