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
            # �J�����g�f�B���N�g�����X�N���v�g���g�̃p�X�ɕύX
            $OldDir = Convert-Path .
            Set-CurrentDirectory $PsDir

            $ComponentName = "AccessDatabaseEngine"

            # �R���|�[�l���g�̃_�E�����[�h
            Download-File (Convert-Path .) "https://download.microsoft.com/download/5/0/F/50FFBB52-334F-4C6D-9727-838CD3CB399E/$ComponentName.exe"

            # �R���|�[�l���g�̃C���X�g�[��
            Start-Process -FilePath .\$ComponentName.exe -ArgumentList "/quiet /log:.\$ComponentName.log" -Wait
            Write-Host "- Install completed"
        }
        catch
        {
            throw $_
        }
        finally
        {
            # �J�����g�f�B���N�g�������ɖ߂�
            Set-CurrentDirectory $OldDir
        }
    }
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
            # ���o����URL����t�@�C���������o��
            $uri = New-Object System.Uri($Url)
            $fileName = Split-Path $uri.AbsolutePath -Leaf

            # �w�肳�ꂽURL����t�@�C�����_�E�����[�h���A�����̃t�@�C�����ŕۑ�
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

# �J�����g�f�B���N�g���ύX
function Set-CurrentDirectory ($path) {
    Set-Location $path
    if ((Get-Location).Provider.Name -eq 'FileSystem') {
        [IO.Directory]::SetCurrentDirectory((Get-Location).ProviderPath)
    }
}

Main -psdir (Split-Path $MyInvocation.MyCommand.Path -Parent)
