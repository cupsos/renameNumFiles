dotnet restore
RID="win10-x64"
dotnet build -r $RID -o $RID
RID="debian.8-x64"
dotnet build -r $RID -o $RID
