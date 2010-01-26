%namespace linqtoweb.CodeGenerator
%scannertype Scanner
%scanbasetype ScanBase
%tokentype Tokens

%option nofiles


num	[0-9]

%%

{num}+		yylval.Object=int.Parse(yytext);return (int)Tokens.DIGIT;

\+			|
\*			|
\/			|
\%			|
\-			return (int)Tokens.OPERATOR;

%%