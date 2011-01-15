cd D:\Prog\Libs\Antlr\
d:
D:\Prog\Libs\Antlr\Antlr3.exe D:\Prog\Projects\AL\Prog\Solution\SimpleOrmFramework\QueryGrammar\SofQuery.g -o D:\Prog\Projects\AL\Prog\Solution\SimpleOrmFramework\QueryGrammar

powershell  "dir D:\Prog\Projects\AL\Prog\Solution\SimpleOrmFramework\QueryGrammar *.cs | foreach-object { (cat $_.FullName) -replace 'public partial class', 'internal partial class' | set-content $_.FullName }"