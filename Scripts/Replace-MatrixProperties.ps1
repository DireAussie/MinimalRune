param (
    [String]
    $Filename
)

(Get-Content $Filename).replace('.M00', '.MXX11') | Set-Content $Filename
(Get-Content $Filename).replace('.M01', '.MXX21') | Set-Content $Filename
(Get-Content $Filename).replace('.M02', '.MXX31') | Set-Content $Filename
(Get-Content $Filename).replace('.M03', '.MXX41') | Set-Content $Filename
(Get-Content $Filename).replace('.M10', '.MXX12') | Set-Content $Filename
(Get-Content $Filename).replace('.M11', '.MXX22') | Set-Content $Filename
(Get-Content $Filename).replace('.M12', '.MXX32') | Set-Content $Filename
(Get-Content $Filename).replace('.M13', '.MXX42') | Set-Content $Filename
(Get-Content $Filename).replace('.M20', '.MXX13') | Set-Content $Filename
(Get-Content $Filename).replace('.M21', '.MXX23') | Set-Content $Filename
(Get-Content $Filename).replace('.M22', '.MXX33') | Set-Content $Filename
(Get-Content $Filename).replace('.M23', '.MXX43') | Set-Content $Filename
(Get-Content $Filename).replace('.M30', '.MXX14') | Set-Content $Filename
(Get-Content $Filename).replace('.M31', '.MXX24') | Set-Content $Filename
(Get-Content $Filename).replace('.M32', '.MXX34') | Set-Content $Filename
(Get-Content $Filename).replace('.M33', '.MXX44') | Set-Content $Filename

(Get-Content $Filename).replace('.MXX11', '.M11') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX21', '.M21') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX31', '.M31') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX41', '.M41') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX12', '.M12') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX22', '.M22') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX32', '.M32') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX42', '.M42') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX13', '.M13') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX23', '.M23') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX33', '.M33') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX43', '.M43') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX14', '.M14') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX24', '.M24') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX34', '.M34') | Set-Content $Filename
(Get-Content $Filename).replace('.MXX44', '.M44') | Set-Content $Filename
