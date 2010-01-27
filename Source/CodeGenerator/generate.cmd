@attrib -r "generated\Lexer.cs"
@attrib -r "generated\Parser.cs"

..\..\Tools\gplex.exe /unicode /babel /out:"generated\Lexer.cs" "generators\Lexer.lex"
..\..\Tools\gppg.exe /gplex /no-lines /babel "generators\Parser.y" > "generated\Parser.cs"

@pause