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
WHITESPACEN				[ \r\t\f]
WHITESPACE              [ \n\r\t\f]
NEWLINE                 ("\r"|"\n"|"\r\n")

%%

/* keywords */

class			{return (int)Tokens.CLASS;}
foreach			{return (int)Tokens.FOREACH;}

/* identifier, numbers, strings */

{IDENTIFIER}	{return (int)Tokens.IDENTIFIER;}
{STRINGVAL}		{return (int)Tokens.STRINGVAL;}
{DNUM}			{return (int)Tokens.DOUBLEVAL;}
{EXPONENT_DNUM}	{return (int)Tokens.DOUBLEVAL;}
{LNUM}			{return (int)Tokens.INTEGERVAL;}
{HNUM}			{return (int)Tokens.INTEGERVAL;}

{WHITESPACEN}+	/* go out with whitespaces, not \n */
//.{NEWLINE}	/* go out with single-line comments */

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