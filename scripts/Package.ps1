function Main
{
    begin
    {
        $ErrorActionPreference = 'stop'

        # スクリプトディレクトリの取得
        $scriptDir = Split-Path $MyInvocation.ScriptName -Parent

        # カレントディレクトリをスクリプト自身のパスに変更
        $OldDir = Convert-Path .
        cd $scriptDir

        $appName = 'WinCap'
        $target = 'Release'
        $bin = '..\source\' + $appName + '\bin\'

        $targetKeywords = '*.exe','*.dll','*.exe.config','*.txt','*.VisualElementsManifest.xml'
        $ignoreKeywords = '*.vshost.*','Microsoft.*.resources.dll'

        $exeSource  = $appName + '.exe'

        if (-not(Test-Path $bin))
        {
            throw 'スクリプトが無効なパスを検出しました。<repository root>\tools-release\が存在していることを確認してください。'
        }
    }

    end
    {
        try
        {
            # clean up current
            Get-ChildItem -Directory | Remove-item -Recurse
            Get-ChildItem | where { $_.Extension -eq ".zip" } | Remove-Item

            Copy-StrictedFilterFileWithDirectoryStructure -Path $(Join-Path $bin $target) -Destination '.\' -Targets $targetKeywords -Exclude $ignoreKeywords

            # valid path check
            $versionSource = Join-Path $target $exeSource -Resolve

            if ((Test-Path $versionSource) -and (Test-Path $target))
            {
                $version = (Get-ChildItem $versionSource).VersionInfo
                $result  = $appName + '-ver.{0}.{1}.{2}' -f $version.ProductMajorPart, $version.ProductMinorPart, $version.ProductBuildPart

                Rename-Item -NewName $result -Path $target
                Compress-Archive -Path $(Join-Path $(Get-Location) $result) -DestinationPath $(Join-Path $(Get-Location).Path ('./' + $result + '.zip'))
            }
        }
        catch
        {
            throw $_
        }
        finally
        {
            # カレントディレクトリを元に戻す
            cd $OldDir
        }
    }
}


# 指定ファイルフィルタでフォルダ構成とともにファイルをコピーする
# https://gist.github.com/guitarrapc/e78bbd4ddc07389e17d6
function Copy-StrictedFilterFileWithDirectoryStructure
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
        $Destination,

        [parameter(
            mandatory = 1,
            position  = 2,
            ValueFromPipelineByPropertyName = 1)]
        [string[]]
        $Targets,

        [parameter(
            mandatory = 0,
            position  = 3,
            ValueFromPipelineByPropertyName = 1)]
        [string[]]
        $Excludes
    )

    begin
    {
        $list = New-Object 'System.Collections.Generic.List[String]'
    }

    process
    {
        Foreach ($target in $Targets)
        {
            # Copy "All Directory Structure" and "File" which Extension type is $ex
            Copy-Item -Path $Path -Destination $Destination -Force -Recurse -Filter $target
        }
    }

    end
    {
        # Remove -Exclude Item
        Foreach ($exclude in $Excludes)
        {
            Get-ChildItem -Path $Destination -Recurse -File | where Name -like $exclude | Remove-Item
        }

        # search Folder which include file
        $allFolder = Get-ChildItem $Destination -Recurse -Directory
        $containsFile = $allFolder | where {$_.GetFiles()}
        $containsFile.FullName `
        | %{
            $fileContains = $_
            $result = $allFolder.FullName `
            | where {$_ -notin $list} `
            | where {
                $shortPath = $_
                $fileContains -like "$shortPath*"
            }
            $result | %{$list.Add($_)}
        }
        $folderToKeep = $list | sort -Unique

        # Remove All Empty (none file exist) folders
        Get-ChildItem -Path $Destination -Recurse -Directory | where fullName -notin $folderToKeep | Remove-Item -Recurse
    }
}

Main
