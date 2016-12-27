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

            $ComponentName = "AccessDatabaseEngine"

            # コンポーネントのダウンロード
            Download-File (Convert-Path .) "https://download.microsoft.com/download/5/0/F/50FFBB52-334F-4C6D-9727-838CD3CB399E/$ComponentName.exe"

            # コンポーネントのインストール
            Start-Process -FilePath .\$ComponentName.exe -ArgumentList "/quiet /log:.\$ComponentName.log" -Wait
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
