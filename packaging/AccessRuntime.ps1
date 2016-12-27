function Main
{
    [CmdletBinding()]
    param
    (
        [parameter(
            mandatory = 1,
            position  = 0,
            ValueFromPipeline = 1,
            ValueFromPipelineByPropertyName = 1)]
        [string]
        $PsDir
    )

    begin
    {
        try
        {
            # カレントディレクトリをスクリプト自身のパスに変更
            $OldDir = Convert-Path .
            Set-CurrentDirectory $PsDir

            $ComponentName = "AccessRuntime"

            # コンポーネントのダウンロード
            Download-File (Convert-Path .) "https://download.microsoft.com/download/9/9/6/996A2380-2842-43F0-BA8A-F80133E6E961/$ComponentName.exe"

            # コンポーネントのインストール
            Start-Process -FilePath .\AccessDatabaseEngine.exe -ArgumentList "/quiet /log:.\$ComponentName.log" -Wait
            Write-Host "- Install completed"
        }
        catch
        {
            throw $_
        }
        finally
        {
            # カレントディレクトリを元に戻す
            Set-CurrentDirectory $OldDir
        }
    }
}

# ファイルダウンロード処理
function Download-File
{
    [CmdletBinding()]
    param
    (
        [parameter(
            mandatory = 1,
            position  = 0,
            ValueFromPipeline = 1,
            ValueFromPipelineByPropertyName = 1)]
        [string]
        $Path,

        [parameter(
            mandatory = 1,
            position  = 1,
            ValueFromPipelineByPropertyName = 1)]
        [string]
        $Url
    )
    
    process
    {
        try
        {
            # 取り出したURLからファイル名を取り出す
            $uri = New-Object System.Uri($Url)
            $fileName = Split-Path $uri.AbsolutePath -Leaf

            # 指定されたURLからファイルをダウンロードし、同名のファイル名で保存
            $client = New-Object System.Net.WebClient
            $client.DownloadFile($Uri, (Join-Path $Path $fileName))
            Write-Host "Downloading `'$fileName`' to $Path From $Url..."
            #Start-BitsTransfer -Source $Url -Destination $Path\$fileName -DisplayName "Downloading `'$fileName`' to $Path" -Priority High -Description "From $Url..." -ErrorVariable err
        }
        catch
        {
            Write-Warning " - An error occurred downloading `'$fileName`'"
            throw $_
        }

        # Pause
        Write-Host "- Downloads completed"
        #Write-Host "- Downloads completed, press any key to exit..."
        #$null = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
}

# カレントディレクトリ変更
function Set-CurrentDirectory ($path) {
    Set-Location $path
    if ((Get-Location).Provider.Name -eq 'FileSystem') {
        [IO.Directory]::SetCurrentDirectory((Get-Location).ProviderPath)
    }
}

Main -psdir (Split-Path $MyInvocation.MyCommand.Path -Parent)
