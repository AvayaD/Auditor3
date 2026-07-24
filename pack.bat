rmdir /S /Q pack
mkdir pack
copy Auditor3\bin\Release\CorruptionAuditor.exe pack\
copy Auditor3\bin\Release\Renci.SshNet.dll pack\
copy Auditor3\bin\Release\Newtonsoft.Json.dll pack\
copy Auditor3\bin\Release\System.Net.Http.Formatting.dll pack\
copy Updater\bin\Release\Updater.exe pack\
copy docs\AuditList.pdf pack\
copy docs\changelog.txt pack\