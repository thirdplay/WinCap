function Main
{
    $ComponentName = "AccessDatabaseEngine"

    # コンポーネントのダウンロード
    Download-File (Convert-Path .) "https://download.microsoft.com/download/5/0/F/50FFBB52-334F-4C6D-9727-838CD3CB399E/$ComponentName.exe"

    # コンポーネントのインストール
    Start-Process -FilePath .\AccessDatabaseEngine.exe -ArgumentList "/quiet /log:.\$ComponentName.log" -Wait
    Write-Host "- Install completed."
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
            $client.DownloadFile($Uri, (Join-Path $Path $file))
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

Main
