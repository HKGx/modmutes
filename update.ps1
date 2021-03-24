git pull
Write-Information "Pulled from origin"
if (-Not $args[0]) {
    throw "You haven't provided the file to download"
}
Invoke-WebRequest $args[0] -OutFile "lista_mute.json"
$date = Get-Item lista_mute.json
| ForEach-Object { $_.LastWriteTimeUtc } 
| ForEach-Object { $_.ToString() }

$_ErrorActionPreference = $ErrorActionPreference
$ErrorActionPreference = "Stop"
Get-Content "lista_mute.json" | ConvertFrom-Json  | ConvertTo-Json  | Out-Null
$ErrorActionPreference = $_ErrorActionPreference

Write-Information "Downloaded and validated the file"
git add lista_mute.json
$str = "Data, gathered at $date UTC"
Write-Output $str
git commit -m "`"$str`""
git push
