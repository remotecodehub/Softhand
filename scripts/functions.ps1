function Show-Spinner {
    param (
        [int]$ProcessId,
        [int]$Delay = 200
    )

    $spinnerChars = @('|', '/', '-', '\', '+')
    $i = 0

    while (Get-Process -Id $ProcessId -ErrorAction SilentlyContinue) {
        Write-Host -NoNewline ("`r[{0}]" -f $spinnerChars[$i]) -ForegroundColor Cyan
        Start-Sleep -Milliseconds $Delay
        $i = ($i + 1) % $spinnerChars.Length
    }

    Write-Host "Done" -ForegroundColor Green
}


function Run-Job {
    param ([Parameter(Mandatory=$true)]$job)
     
    while ($job.State -eq 'Running') {
        Show-Spinner -ProcessId $job.Id
        Start-Sleep -Milliseconds 200
    }

    Receive-Job $job
    Remove-Job -Force $job
}
