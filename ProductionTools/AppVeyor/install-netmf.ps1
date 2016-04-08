Write-Host "Installing .NET MicroFramework 4.3 ..."
$msiPath = "$($env:USERPROFILE)\MicroFrameworkSDK43.MSI"
$downloadSource = "http://cdn.netduino.com/downloads/netmfsdk/v4.3.2-QFE2/MicroFrameworkSDK.MSI"
Write-Host "Downloading from $downloadSource to $msiPath ..."
(New-Object Net.WebClient).DownloadFile($downloadSource, $msiPath)
Write-Host "Download completed."
Write-Host "Installing ..."
msiexec /i $msiPath /quiet | Out-default
Write-Host "Installed." -ForegroundColor green 
