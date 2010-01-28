%namespace linqtoweb.CodeGenerator
%using linqtoweb.CodeGenerator.AST;
%scannertype Scanner
%scanbasetype ScanBase
%tokentype Tokens

%option nofiles

%x comment

HexDigit                [0-9A-Fa-f]
StringChar				[^\r\n\"\\]|[\\.]

LNUM                    [0-9]+
DNUM                    ([0-9]*[.][0-9]+)|([0-9]+[.][0-9]*)
EXPONENT_DNUM           (({LNUM}|{DNUM})[eE][\+\-]\?{LNUM})
HNUM                    "0x"{HexDigit}+
STRINGVAL				\"{StringChar}*\"
IDENTIFIER              [a-zA-Z_][a-zA-Z0-9_]*
NAMESPACE				{IDENTIFIER}(\.{IDENTIFIER})*
WHITESPACE              [ \n\r\t\f]
NEWLINE                 ("\r"|"\n"|"\r\n")

%%

/* keywords */

class			{return (int)Tokens.CLASS;}
foreach			{return (int)Tokens.FOREACH;}

string			{return (int)Tokens.TSTRING;}
int				{return (int)Tokens.TINT;}
double			{return (int)Tokens.TDOUBLE;}
datetime		{return (int)Tokens.TDATETIME;}

/* identifier, numbers, strings */

{IDENTIFIER}	{yylval.obj = yytext; return (int)Tokens.IDENTIFIER;}
{STRINGVAL}		{yylval.obj = yytext; return (int)Tokens.STRINGVAL;}
{NAMESPACE}		{yylval.obj = yytext; return (int)Tokens.DOTTEDIDENTIFIER;}

{DNUM}			{yylval.obj = double.Parse(yytext); return (int)Tokens.DOUBLEVAL;}
{EXPONENT_DNUM}	{yylval.obj = double.Parse(yytext); return (int)Tokens.DOUBLEVAL;}

{LNUM}			{yylval.obj = int.Parse(yytext); return (int)Tokens.INTEGERVAL;}
{HNUM}			{yylval.obj = int.Parse(yytext); return (int)Tokens.INTEGERVAL;}

{WHITESPACE}+	/* skip whitespaces */
//.{NEWLINE}	/* skip single-line comments */

/* symbols, operators */

\[				{return (int)Tokens.LBRACKET;}
\]				{return (int)Tokens.RBRACKET;}
\{				{return (int)Tokens.LBRACE;}
\}				{return (int)Tokens.RBRACE;}
\(				{return (int)Tokens.LPAREN;}
\)				{return (int)Tokens.RPAREN;}
\:				{return (int)Tokens.COMMA;}
\;				{return (int)Tokens.SEMICOLON;}
\+				{return (int)Tokens.OP_PLUS;}
\-				{return (int)Tokens.OP_MINUS;}
\*				{return (int)Tokens.OP_MUL;}
\/				{return (int)Tokens.OP_DIV;}
\=				{return (int)Tokens.OP_ASSIGN;}


/* comments */

\/\*			{ BEGIN(comment); }
<comment>{
[^(\*\/)]+		/* comment text, ignored */
\*\/			{ BEGIN(INITIAL); }
}


%%