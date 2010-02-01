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
DOTTEDIDENTIFIER		{IDENTIFIER}(\.{IDENTIFIER})+
WHITESPACE              [ \n\r\t\f]
NEWLINE                 ("\r"|"\n"|"\r\n")

%%

/* symbols, operators */

\[				{return (int)Tokens.LBRACKET;}
\]				{return (int)Tokens.RBRACKET;}
\{				{return (int)Tokens.LBRACE;}
\}				{return (int)Tokens.RBRACE;}
\(				{return (int)Tokens.LPAREN;}
\)				{return (int)Tokens.RPAREN;}
\,				{return (int)Tokens.COMMA;}
\;				{return (int)Tokens.SEMICOLON;}

\+\+			{return (int)Tokens.OP_ADD1;}
\-\-			{return (int)Tokens.OP_SUB1;}
\+				{return (int)Tokens.OP_ADD;}
\-				{return (int)Tokens.OP_SUB;}
\*				{return (int)Tokens.OP_MUL;}
\/				{return (int)Tokens.OP_DIV;}
\=				{return (int)Tokens.OP_ASSIGN;}
\?				{return (int)Tokens.OP_QUESTION;}
\:				{return (int)Tokens.OP_COLON;}

\|\|			{return (int)Tokens.OP_LOGIC_OR;}
or				{return (int)Tokens.OP_LOGIC_OR;}
\&\&			{return (int)Tokens.OP_LOGIC_AND;}
and				{return (int)Tokens.OP_LOGIC_AND;}
xor				{return (int)Tokens.OP_LOGIC_XOR;}
\!				{return (int)Tokens.OP_LOGIC_NOT;}
not				{return (int)Tokens.OP_LOGIC_NOT;}
\=\=			{return (int)Tokens.OP_EQ;}
\!\=			{return (int)Tokens.OP_NOTEQ;}
\<				{return (int)Tokens.OP_LESS;}
\>				{return (int)Tokens.OP_GREATER;}
\<\=			{return (int)Tokens.OP_LESSEQ;}
\>\=			{return (int)Tokens.OP_GREATEREQ;}

/* keywords */

class			{return (int)Tokens.CLASS;}
foreach			{return (int)Tokens.FOREACH;}

string			{return (int)Tokens.TSTRING;}
int				{return (int)Tokens.TINT;}
double			{return (int)Tokens.TDOUBLE;}
datetime		{return (int)Tokens.TDATETIME;}
bool			{return (int)Tokens.TBOOL;}

true			{yylval.obj = true; return (int)Tokens.BOOLVAL;}
false			{yylval.obj = false; return (int)Tokens.BOOLVAL;}

/* identifier, numbers, strings */

{DNUM}			{yylval.obj = double.Parse(yytext); return (int)Tokens.DOUBLEVAL;}
{EXPONENT_DNUM}	{yylval.obj = double.Parse(yytext); return (int)Tokens.DOUBLEVAL;}

{LNUM}			{yylval.obj = int.Parse(yytext); return (int)Tokens.INTEGERVAL;}
{HNUM}			{yylval.obj = int.Parse(yytext); return (int)Tokens.INTEGERVAL;}

{IDENTIFIER}	{yylval.obj = yytext; return (int)Tokens.IDENTIFIER;}
{DOTTEDIDENTIFIER}		{yylval.obj = yytext; return (int)Tokens.DOTTEDIDENTIFIER;}
{STRINGVAL}		{yylval.obj = yytext; return (int)Tokens.STRINGVAL;}


{WHITESPACE}+	/* skip whitespaces */
//.{NEWLINE}	/* skip single-line comments */

/* comments */

\/\*			{ BEGIN(comment); }
<comment>{
[^(\*\/)]+		/* comment text, ignored */
\*\/			{ BEGIN(INITIAL); }
}


%%

	public override ExprPosition yylloc { get { return new ExprPosition(tokLin,tokCol,tokELin,tokECol); } set {  } }