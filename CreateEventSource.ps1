$eventSource = "Fujitsu SLM Web UI";
if ([System.Diagnostics.EventLog]::SourceExists($eventSource) -eq $false) {
	Write-Host "Create the Event Source '$eventSource'"
	[System.Diagnostics.EventLog]::CreateEventSource($eventSource, "Application")
}
else
{
	Write-Host "Event Source '$eventSource' already exists on this platform"
}