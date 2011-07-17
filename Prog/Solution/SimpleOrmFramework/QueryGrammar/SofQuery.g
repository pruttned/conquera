grammar SofQuery;
 

options
{
    language=CSharp2;
    output=AST;
    ASTLabelType=CommonTree;
}



 

tokens
{
	EXPR;
	ID;
}

@lexer::namespace { SimpleOrmFramework }
@parser::namespace { SimpleOrmFramework }


start 	:	query EOF
	;

query	:	expr ((AND|OR) query)?
	|	'('query')' ((AND|OR) query)?
	;

expr    :	expr2 -> ^(EXPR expr2)
	;

expr2 	:	identifier('='|'<'|'>'|'<='|'>='|'<>')VALUE 
	;


identifier : (SUBIDENTIFIER ('.'SUBIDENTIFIER) * ) -> ^(ID SUBIDENTIFIER (SUBIDENTIFIER) * )
	;
	
AND 	:	 ('a'|'A')('n'|'N')('d'|'D')
	;
OR 	:	 ('o'|'O')('r'|'R')
	;	
	
SUBIDENTIFIER :	LETTER (LETTER|'0'..'9')* 
	;

VALUE 	:	('0'..'9')+('.'('0'..'9')+)?
	|	'\''( options {greedy=false;} : . )*'\''
	;

fragment	
LETTER 	:	'A'..'Z'
	|	'a'..'z'
	|	'_'
	;

WS  :  (' '|'\r'|'\t'|'\u000C'|'\n') {$channel=HIDDEN;}
    ;

