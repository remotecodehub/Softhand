param (
    [Parameter(Mandatory = $true, ParameterSetName = "Android")]
    [switch]$android,

    [Parameter(Mandatory = $true, ParameterSetName = "iOS")]
    [switch]$ios,

    [Parameter(Mandatory = $true)]
    [string]$abi
)

# ✅ Lista de ABIs válidas
$validAbis = @(
    "ios-x64", "ios-arm64", "iossimulator-arm64",
    "android-arm", "android-arm64", "android-x86", "android-x86_64"
)

if (-not $validAbis.Contains($abi)) {
    Write-Error "❌ ABI '$abi' inválida. Use uma das seguintes: $($validAbis -join ', ')"
    exit 1
}

$dotnet = "dotnet"

# 🟢 Execução Android
if ($android) {
    Write-Host "📱 Plataforma: Android"
    $emulatorPath = "$env:ANDROID_HOME/emulator/emulator"
    $adb = "$env:ANDROID_HOME/platform-tools/adb.exe"

    $emulatorAbis = @("android-x86", "android-x86_64")
    $deviceAbis = @("android-arm", "android-arm64")

    if ($emulatorAbis.Contains($abi)) {
        $avds = & $emulatorPath -list-avds
        $matchingAvd = $avds | Where-Object {
            $configPath = "$env:USERPROFILE\.android\avd\$_.avd\config.ini"
            if (Test-Path $configPath) {
                $abiLine = Get-Content $configPath | Where-Object { $_ -match "^abi\.type=" }
                $abiType = $abiLine -replace "abi\.type=", ""
                return $abiType -eq $abi.Replace("android-", "")
            }
        } | Select-Object -First 1

        if (-not $matchingAvd) {
            Write-Host "⚠️ Nenhum AVD compatível com ABI '$abi'."
            $newAvdName = Read-Host "Digite um nome para o novo AVD"
            $systemImage = Read-Host "Digite o caminho do system-image para ABI '$abi' (ex: system-images;android-34;google_apis;x86_64)"
            & "$env:ANDROID_HOME/cmdline-tools/latest/bin/avdmanager.bat" create avd -n $newAvdName -k $systemImage
            $matchingAvd = $newAvdName
        }

        Write-Host "🚀 Iniciando emulador '$matchingAvd'..."
        Start-Process $emulatorPath -ArgumentList "@$matchingAvd"
        Start-Sleep -Seconds 10
        & $adb wait-for-device

        $deviceId = (& $adb devices | Select-String "emulator-" | ForEach-Object { ($_ -split "\s+")[0] })[0]
        Write-Host "📦 Executando dotnet maui run no emulador '$deviceId'..."
        & $dotnet maui run -f net9.0-android -c Debug -r $abi --device $deviceId

    } elseif ($deviceAbis.Contains($abi)) {
        Write-Host "🔌 Conecte seu dispositivo Android com ABI '$abi' via USB."
        & $adb wait-for-device
        $deviceId = (& $adb devices | Select-String -NotMatch "emulator-" | ForEach-Object { ($_ -split "\s+")[0] })[0]
        if (-not $deviceId) {
            Write-Error "❌ Nenhum dispositivo Android conectado."
            exit 1
        }
        Write-Host "📦 Executando dotnet maui run no dispositivo '$deviceId'..."
        & $dotnet maui run -f net9.0-android -c Debug -r $abi --device $deviceId
    }
}

# 🍏 Execução iOS
if ($ios) {
    Write-Host "🍏 Plataforma: iOS"
    $user = Read-Host "👤 Usuário do Mac"
    $rhost = Read-Host "🌐 Host/IP do Mac"
    $password = Read-Host "🔐 Senha do Mac"

    Write-Host "🔗 Conectando ao Mac via SSH..."
    $session = New-PSSession -HostName $rhost -UserName $user -Password $password

    if ($abi -eq "iossimulator-arm64" -or $abi -eq "ios-x64") {
        $simulators = Invoke-Command -Session $session -ScriptBlock { xcrun simctl list devices available }
        $simId = ($simulators | Select-String -Pattern "Booted" | Select-String -Pattern ($abi.Split("-")[1]) | ForEach-Object {
            ($_ -split "\(|\)")[1]
        })[0]

        if (-not $simId) {
            Write-Error "❌ Nenhum simulador iOS disponível com ABI '$abi'."
            exit 1
        }

        Write-Host "📦 Executando dotnet maui run no simulador '$simId'..."
        Invoke-Command -Session $session -ScriptBlock {
            dotnet maui run -f net9.0-ios -c Debug -r $using:abi --device $using:simId
        }

    } elseif ($abi -eq "ios-arm64") {
        $devices = Invoke-Command -Session $session -ScriptBlock { idevice_id -l }
        $deviceId = $devices[0]
        if (-not $deviceId) {
            Write-Error "❌ Nenhum dispositivo iOS conectado ao Mac."
            exit 1
        }

        Write-Host "📦 Executando dotnet maui run no dispositivo '$deviceId'..."
        Invoke-Command -Session $session -ScriptBlock {
            dotnet maui run -f net9.0-ios -c Debug -r $using:abi --device $using:deviceId
        }
    }

    Remove-PSSession $session
} 