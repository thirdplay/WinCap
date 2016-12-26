function Main
{
    $ComponentName = "AccessDatabaseEngine"

    # �R���|�[�l���g�̃_�E�����[�h
    Download-File (Convert-Path .) "https://download.microsoft.com/download/5/0/F/50FFBB52-334F-4C6D-9727-838CD3CB399E/$ComponentName.exe"

    # �R���|�[�l���g�̃C���X�g�[��
    Start-Process -FilePath .\AccessDatabaseEngine.exe -ArgumentList "/quiet /log:.\$ComponentName.log" -Wait
    Write-Host "- Install completed."
}

# �t�@�C���_�E�����[�h����
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
            Import-Module BitsTransfer

            # ���o����URL����t�@�C���������o��
            $uri = New-Object System.Uri($Url)
            $destFileName = Split-Path $uri.AbsolutePath -Leaf

            # �w�肳�ꂽURL����t�@�C�����_�E�����[�h���A�����̃t�@�C�����ŕۑ�
            Start-BitsTransfer -Source $Url -Destination $Path\$destFileName -DisplayName "Downloading `'$destFileName`' to $Path" -Priority High -Description "From $Url..." -ErrorVariable err
        }
        catch
        {
            Write-Warning " - An error occurred downloading `'$destFileName`'"
            throw $_
        }

        # Pause
        Write-Host "- Downloads completed"
        #Write-Host "- Downloads completed, press any key to exit..."
        #$null = $host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
    }
}

Main
