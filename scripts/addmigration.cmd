pushd ..\src\OrcShackApi.Web
dotnet ef --project ../OrcShackApi.Data --startup-project . migrations add %1 --context OrcShackApiContext
popd
pause