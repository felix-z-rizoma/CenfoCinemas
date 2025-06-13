# Obtener fechas únicas de commits (ordenadas)
$dates = git log --date=short --pretty=format:"%ad" | Sort-Object | Get-Unique

foreach ($date in $dates) {
    Write-Host "${date}: " -NoNewline

    # Obtener estadísticas de líneas agregadas/eliminadas para ese día
    $stats = git log --since="$date 00:00" --until="$date 23:59" --pretty=tformat: --numstat

    $added = 0
    $removed = 0

    foreach ($line in $stats) {
        if ($line -match "^\d+") {
            # Split the line by tabs, and ensure we are getting two columns
            $cols = $line -split "`t"
            if ($cols.Length -ge 2) {
                # Safely convert the values to integers and accumulate them
                $added += [int]$cols[0]
                $removed += [int]$cols[1]
            }
        }
    }

    # Output the results for the date
    Write-Host "agregadas: $added, eliminadas: $removed"
}
