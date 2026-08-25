rm -rf ./pub-linux
rm -rf ./pub-windows
rm -rf ./pub-mac
rm -rf ./pub-html
dotnet publish ./src/Metaphor.Spectre/Metaphor.Spectre.vbproj -o ./pub-linux -c Release --sc -p:PublishSingleFile=true -r linux-x64
dotnet publish ./src/Metaphor.Spectre/Metaphor.Spectre.vbproj -o ./pub-windows -c Release --sc -p:PublishSingleFile=true -r win-x64
dotnet publish ./src/Metaphor.Spectre/Metaphor.Spectre.vbproj -o ./pub-mac -c Release --sc -p:PublishSingleFile=true -r osx-x64
dotnet publish ./src/Metaphor.Blazor/Metaphor.Blazor.csproj -o ./pub-html -c Release 
rm -f ./pub-linux/*.pdb
rm -f ./pub-windows/*.pdb
rm -f ./pub-mac/*.pdb
rm -f ./pub-html/*.pdb
butler push pub-windows thegrumpygamedev/shark-attackers-of-splorr:windows
butler push pub-linux thegrumpygamedev/shark-attackers-of-splorr:linux
butler push pub-mac thegrumpygamedev/shark-attackers-of-splorr:mac
butler push pub-html/wwwroot thegrumpygamedev/shark-attackers-of-splorr:html
