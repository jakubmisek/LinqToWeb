%namespace linqtoweb.CodeGenerator
%parsertype Parser
%tokentype Tokens
//%valuetype object

%token DIGIT OPERATOR

%left OPERATOR

%start init

%union
{
	public object Object;
	public LexLocation Pos;
}

%%

init	:	expr
				{
					Result = $1.Object;
				}
		;
		
expr    :   '(' expr ')'
                {
                    $$.Object = $2.Object;
                    $$.Pos = @1.Merge(@3);
                }
        |   expr OPERATOR expr
                {
                    $$.Object = null;//new OP($2.Object, $1.Object, $3.Object);
                    $$.Pos = @1.Merge(@3);
                }
        |   number
        ;

number  :   DIGIT
                {
                    $$.Object = $1.Object;
                    $$.Pos = @1;
                }
        ;

%%

    public Parser(Scanner scanner) : base(scanner) { }
    
    public object Result {get;private set;}

