//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Etrak Studios <etrakstudios@gmail.com >
//  Part of the Conquera Project
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 2 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.
////////////////////////////////////////////////////////////////////////

// $ANTLR 3.1.2 D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g 2009-12-23 12:47:48

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162


using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;



using Antlr.Runtime.Tree;

namespace  SimpleOrmFramework 
{
internal partial class SofQueryParser : Parser
{
    public static readonly string[] tokenNames = new string[] 
	{
        "<invalid>", 
		"<EOR>", 
		"<DOWN>", 
		"<UP>", 
		"AND", 
		"EXPR", 
		"ID", 
		"LETTER", 
		"OR", 
		"SUBIDENTIFIER", 
		"VALUE", 
		"WS", 
		"'('", 
		"')'", 
		"'.'", 
		"'<'", 
		"'<='", 
		"'<>'", 
		"'='", 
		"'>'", 
		"'>='"
    };

    public const int EOF = -1;
    public const int T__12 = 12;
    public const int T__13 = 13;
    public const int T__14 = 14;
    public const int T__15 = 15;
    public const int T__16 = 16;
    public const int T__17 = 17;
    public const int T__18 = 18;
    public const int T__19 = 19;
    public const int T__20 = 20;
    public const int AND = 4;
    public const int EXPR = 5;
    public const int ID = 6;
    public const int LETTER = 7;
    public const int OR = 8;
    public const int SUBIDENTIFIER = 9;
    public const int VALUE = 10;
    public const int WS = 11;

    // delegates
    // delegators



        public SofQueryParser(ITokenStream input)
    		: this(input, new RecognizerSharedState()) {
        }

        public SofQueryParser(ITokenStream input, RecognizerSharedState state)
    		: base(input, state) {
            InitializeCyclicDFAs();

             
        }
        
    protected ITreeAdaptor adaptor = new CommonTreeAdaptor();

    public ITreeAdaptor TreeAdaptor
    {
        get { return this.adaptor; }
        set {
    	this.adaptor = value;
    	}
    }

    override public string[] TokenNames {
		get { return SofQueryParser.tokenNames; }
    }

    override public string GrammarFileName {
		get { return "D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g"; }
    }


    public class start_return : ParserRuleReturnScope
    {
        private CommonTree tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (CommonTree) value; }
        }
    };

    // $ANTLR start "start"
    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:25:0: start : query EOF ;
    public SofQueryParser.start_return start() // throws RecognitionException [1]
    {   
        SofQueryParser.start_return retval = new SofQueryParser.start_return();
        retval.Start = input.LT(1);

        CommonTree root_0 = null;

        IToken EOF2 = null;
        SofQueryParser.query_return query1 = default(SofQueryParser.query_return);


        CommonTree EOF2_tree=null;

        try 
    	{
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:25:10: ( query EOF )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:25:10: query EOF
            {
            	root_0 = (CommonTree)adaptor.GetNilNode();

            	PushFollow(FOLLOW_query_in_start81);
            	query1 = query();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, query1.Tree);
            	EOF2=(IToken)Match(input,EOF,FOLLOW_EOF_in_start83); 
            		EOF2_tree = (CommonTree)adaptor.Create(EOF2);
            		adaptor.AddChild(root_0, EOF2_tree);


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (CommonTree)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "start"

    public class query_return : ParserRuleReturnScope
    {
        private CommonTree tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (CommonTree) value; }
        }
    };

    // $ANTLR start "query"
    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:28:0: query : ( expr ( ( AND | OR ) query )? | '(' query ')' ( ( AND | OR ) query )? );
    public SofQueryParser.query_return query() // throws RecognitionException [1]
    {   
        SofQueryParser.query_return retval = new SofQueryParser.query_return();
        retval.Start = input.LT(1);

        CommonTree root_0 = null;

        IToken set4 = null;
        IToken char_literal6 = null;
        IToken char_literal8 = null;
        IToken set9 = null;
        SofQueryParser.expr_return expr3 = default(SofQueryParser.expr_return);

        SofQueryParser.query_return query5 = default(SofQueryParser.query_return);

        SofQueryParser.query_return query7 = default(SofQueryParser.query_return);

        SofQueryParser.query_return query10 = default(SofQueryParser.query_return);


        CommonTree set4_tree=null;
        CommonTree char_literal6_tree=null;
        CommonTree char_literal8_tree=null;
        CommonTree set9_tree=null;

        try 
    	{
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:28:9: ( expr ( ( AND | OR ) query )? | '(' query ')' ( ( AND | OR ) query )? )
            int alt3 = 2;
            int LA3_0 = input.LA(1);

            if ( (LA3_0 == SUBIDENTIFIER) )
            {
                alt3 = 1;
            }
            else if ( (LA3_0 == 12) )
            {
                alt3 = 2;
            }
            else 
            {
                NoViableAltException nvae_d3s0 =
                    new NoViableAltException("", 3, 0, input);

                throw nvae_d3s0;
            }
            switch (alt3) 
            {
                case 1 :
                    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:28:9: expr ( ( AND | OR ) query )?
                    {
                    	root_0 = (CommonTree)adaptor.GetNilNode();

                    	PushFollow(FOLLOW_expr_in_query93);
                    	expr3 = expr();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, expr3.Tree);
                    	// D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:28:14: ( ( AND | OR ) query )?
                    	int alt1 = 2;
                    	int LA1_0 = input.LA(1);

                    	if ( (LA1_0 == AND || LA1_0 == OR) )
                    	{
                    	    alt1 = 1;
                    	}
                    	switch (alt1) 
                    	{
                    	    case 1 :
                    	        // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:28:15: ( AND | OR ) query
                    	        {
                    	        	set4 = (IToken)input.LT(1);
                    	        	if ( input.LA(1) == AND || input.LA(1) == OR ) 
                    	        	{
                    	        	    input.Consume();
                    	        	    adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set4));
                    	        	    state.errorRecovery = false;
                    	        	}
                    	        	else 
                    	        	{
                    	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        	    throw mse;
                    	        	}

                    	        	PushFollow(FOLLOW_query_in_query102);
                    	        	query5 = query();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, query5.Tree);

                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:29:4: '(' query ')' ( ( AND | OR ) query )?
                    {
                    	root_0 = (CommonTree)adaptor.GetNilNode();

                    	char_literal6=(IToken)Match(input,12,FOLLOW_12_in_query109); 
                    		char_literal6_tree = (CommonTree)adaptor.Create(char_literal6);
                    		adaptor.AddChild(root_0, char_literal6_tree);

                    	PushFollow(FOLLOW_query_in_query110);
                    	query7 = query();
                    	state.followingStackPointer--;

                    	adaptor.AddChild(root_0, query7.Tree);
                    	char_literal8=(IToken)Match(input,13,FOLLOW_13_in_query111); 
                    		char_literal8_tree = (CommonTree)adaptor.Create(char_literal8);
                    		adaptor.AddChild(root_0, char_literal8_tree);

                    	// D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:29:16: ( ( AND | OR ) query )?
                    	int alt2 = 2;
                    	int LA2_0 = input.LA(1);

                    	if ( (LA2_0 == AND || LA2_0 == OR) )
                    	{
                    	    alt2 = 1;
                    	}
                    	switch (alt2) 
                    	{
                    	    case 1 :
                    	        // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:29:17: ( AND | OR ) query
                    	        {
                    	        	set9 = (IToken)input.LT(1);
                    	        	if ( input.LA(1) == AND || input.LA(1) == OR ) 
                    	        	{
                    	        	    input.Consume();
                    	        	    adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set9));
                    	        	    state.errorRecovery = false;
                    	        	}
                    	        	else 
                    	        	{
                    	        	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        	    throw mse;
                    	        	}

                    	        	PushFollow(FOLLOW_query_in_query120);
                    	        	query10 = query();
                    	        	state.followingStackPointer--;

                    	        	adaptor.AddChild(root_0, query10.Tree);

                    	        }
                    	        break;

                    	}


                    }
                    break;

            }
            retval.Stop = input.LT(-1);

            	retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (CommonTree)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "query"

    public class expr_return : ParserRuleReturnScope
    {
        private CommonTree tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (CommonTree) value; }
        }
    };

    // $ANTLR start "expr"
    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:32:0: expr : expr2 -> ^( EXPR expr2 ) ;
    public SofQueryParser.expr_return expr() // throws RecognitionException [1]
    {   
        SofQueryParser.expr_return retval = new SofQueryParser.expr_return();
        retval.Start = input.LT(1);

        CommonTree root_0 = null;

        SofQueryParser.expr2_return expr211 = default(SofQueryParser.expr2_return);


        RewriteRuleSubtreeStream stream_expr2 = new RewriteRuleSubtreeStream(adaptor,"rule expr2");
        try 
    	{
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:32:11: ( expr2 -> ^( EXPR expr2 ) )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:32:11: expr2
            {
            	PushFollow(FOLLOW_expr2_in_expr135);
            	expr211 = expr2();
            	state.followingStackPointer--;

            	stream_expr2.Add(expr211.Tree);


            	// AST REWRITE
            	// elements:          expr2
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (CommonTree)adaptor.GetNilNode();
            	// 32:17: -> ^( EXPR expr2 )
            	{
            	    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:32:20: ^( EXPR expr2 )
            	    {
            	    CommonTree root_1 = (CommonTree)adaptor.GetNilNode();
            	    root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(EXPR, "EXPR"), root_1);

            	    adaptor.AddChild(root_1, stream_expr2.NextTree());

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (CommonTree)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "expr"

    public class expr2_return : ParserRuleReturnScope
    {
        private CommonTree tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (CommonTree) value; }
        }
    };

    // $ANTLR start "expr2"
    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:35:0: expr2 : identifier ( '=' | '<' | '>' | '<=' | '>=' | '<>' ) VALUE ;
    public SofQueryParser.expr2_return expr2() // throws RecognitionException [1]
    {   
        SofQueryParser.expr2_return retval = new SofQueryParser.expr2_return();
        retval.Start = input.LT(1);

        CommonTree root_0 = null;

        IToken set13 = null;
        IToken VALUE14 = null;
        SofQueryParser.identifier_return identifier12 = default(SofQueryParser.identifier_return);


        CommonTree set13_tree=null;
        CommonTree VALUE14_tree=null;

        try 
    	{
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:35:10: ( identifier ( '=' | '<' | '>' | '<=' | '>=' | '<>' ) VALUE )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:35:10: identifier ( '=' | '<' | '>' | '<=' | '>=' | '<>' ) VALUE
            {
            	root_0 = (CommonTree)adaptor.GetNilNode();

            	PushFollow(FOLLOW_identifier_in_expr2154);
            	identifier12 = identifier();
            	state.followingStackPointer--;

            	adaptor.AddChild(root_0, identifier12.Tree);
            	set13 = (IToken)input.LT(1);
            	if ( (input.LA(1) >= 15 && input.LA(1) <= 20) ) 
            	{
            	    input.Consume();
            	    adaptor.AddChild(root_0, (CommonTree)adaptor.Create(set13));
            	    state.errorRecovery = false;
            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    throw mse;
            	}

            	VALUE14=(IToken)Match(input,VALUE,FOLLOW_VALUE_in_expr2168); 
            		VALUE14_tree = (CommonTree)adaptor.Create(VALUE14);
            		adaptor.AddChild(root_0, VALUE14_tree);


            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (CommonTree)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "expr2"

    public class identifier_return : ParserRuleReturnScope
    {
        private CommonTree tree;
        override public object Tree
        {
        	get { return tree; }
        	set { tree = (CommonTree) value; }
        }
    };

    // $ANTLR start "identifier"
    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:39:0: identifier : ( SUBIDENTIFIER ( '.' SUBIDENTIFIER )* ) -> ^( ID SUBIDENTIFIER ( SUBIDENTIFIER )* ) ;
    public SofQueryParser.identifier_return identifier() // throws RecognitionException [1]
    {   
        SofQueryParser.identifier_return retval = new SofQueryParser.identifier_return();
        retval.Start = input.LT(1);

        CommonTree root_0 = null;

        IToken SUBIDENTIFIER15 = null;
        IToken char_literal16 = null;
        IToken SUBIDENTIFIER17 = null;

        CommonTree SUBIDENTIFIER15_tree=null;
        CommonTree char_literal16_tree=null;
        CommonTree SUBIDENTIFIER17_tree=null;
        RewriteRuleTokenStream stream_SUBIDENTIFIER = new RewriteRuleTokenStream(adaptor,"token SUBIDENTIFIER");
        RewriteRuleTokenStream stream_14 = new RewriteRuleTokenStream(adaptor,"token 14");

        try 
    	{
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:39:14: ( ( SUBIDENTIFIER ( '.' SUBIDENTIFIER )* ) -> ^( ID SUBIDENTIFIER ( SUBIDENTIFIER )* ) )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:39:14: ( SUBIDENTIFIER ( '.' SUBIDENTIFIER )* )
            {
            	// D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:39:14: ( SUBIDENTIFIER ( '.' SUBIDENTIFIER )* )
            	// D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:39:15: SUBIDENTIFIER ( '.' SUBIDENTIFIER )*
            	{
            		SUBIDENTIFIER15=(IToken)Match(input,SUBIDENTIFIER,FOLLOW_SUBIDENTIFIER_in_identifier181);  
            		stream_SUBIDENTIFIER.Add(SUBIDENTIFIER15);

            		// D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:39:29: ( '.' SUBIDENTIFIER )*
            		do 
            		{
            		    int alt4 = 2;
            		    int LA4_0 = input.LA(1);

            		    if ( (LA4_0 == 14) )
            		    {
            		        alt4 = 1;
            		    }


            		    switch (alt4) 
            			{
            				case 1 :
            				    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:39:30: '.' SUBIDENTIFIER
            				    {
            				    	char_literal16=(IToken)Match(input,14,FOLLOW_14_in_identifier184);  
            				    	stream_14.Add(char_literal16);

            				    	SUBIDENTIFIER17=(IToken)Match(input,SUBIDENTIFIER,FOLLOW_SUBIDENTIFIER_in_identifier185);  
            				    	stream_SUBIDENTIFIER.Add(SUBIDENTIFIER17);


            				    }
            				    break;

            				default:
            				    goto loop4;
            		    }
            		} while (true);

            		loop4:
            			;	// Stops C# compiler whining that label 'loop4' has no statements


            	}



            	// AST REWRITE
            	// elements:          SUBIDENTIFIER, SUBIDENTIFIER
            	// token labels:      
            	// rule labels:       retval
            	// token list labels: 
            	// rule list labels:  
            	// wildcard labels: 
            	retval.Tree = root_0;
            	RewriteRuleSubtreeStream stream_retval = new RewriteRuleSubtreeStream(adaptor, "rule retval", retval!=null ? retval.Tree : null);

            	root_0 = (CommonTree)adaptor.GetNilNode();
            	// 39:52: -> ^( ID SUBIDENTIFIER ( SUBIDENTIFIER )* )
            	{
            	    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:39:55: ^( ID SUBIDENTIFIER ( SUBIDENTIFIER )* )
            	    {
            	    CommonTree root_1 = (CommonTree)adaptor.GetNilNode();
            	    root_1 = (CommonTree)adaptor.BecomeRoot((CommonTree)adaptor.Create(ID, "ID"), root_1);

            	    adaptor.AddChild(root_1, stream_SUBIDENTIFIER.NextNode());
            	    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:39:74: ( SUBIDENTIFIER )*
            	    while ( stream_SUBIDENTIFIER.HasNext() )
            	    {
            	        adaptor.AddChild(root_1, stream_SUBIDENTIFIER.NextNode());

            	    }
            	    stream_SUBIDENTIFIER.Reset();

            	    adaptor.AddChild(root_0, root_1);
            	    }

            	}

            	retval.Tree = root_0;retval.Tree = root_0;
            }

            retval.Stop = input.LT(-1);

            	retval.Tree = (CommonTree)adaptor.RulePostProcessing(root_0);
            	adaptor.SetTokenBoundaries(retval.Tree, (IToken) retval.Start, (IToken) retval.Stop);
        }
        catch (RecognitionException re) 
    	{
            ReportError(re);
            Recover(input,re);
    	// Conversion of the second argument necessary, but harmless
    	retval.Tree = (CommonTree)adaptor.ErrorNode(input, (IToken) retval.Start, input.LT(-1), re);

        }
        finally 
    	{
        }
        return retval;
    }
    // $ANTLR end "identifier"

    // Delegated rules


	private void InitializeCyclicDFAs()
	{
	}

 

    public static readonly BitSet FOLLOW_query_in_start81 = new BitSet(new ulong[]{0x0000000000000000UL});
    public static readonly BitSet FOLLOW_EOF_in_start83 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr_in_query93 = new BitSet(new ulong[]{0x0000000000000112UL});
    public static readonly BitSet FOLLOW_set_in_query96 = new BitSet(new ulong[]{0x0000000000001200UL});
    public static readonly BitSet FOLLOW_query_in_query102 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_12_in_query109 = new BitSet(new ulong[]{0x0000000000001200UL});
    public static readonly BitSet FOLLOW_query_in_query110 = new BitSet(new ulong[]{0x0000000000002000UL});
    public static readonly BitSet FOLLOW_13_in_query111 = new BitSet(new ulong[]{0x0000000000000112UL});
    public static readonly BitSet FOLLOW_set_in_query114 = new BitSet(new ulong[]{0x0000000000001200UL});
    public static readonly BitSet FOLLOW_query_in_query120 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_expr2_in_expr135 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_identifier_in_expr2154 = new BitSet(new ulong[]{0x00000000001F8000UL});
    public static readonly BitSet FOLLOW_set_in_expr2155 = new BitSet(new ulong[]{0x0000000000000400UL});
    public static readonly BitSet FOLLOW_VALUE_in_expr2168 = new BitSet(new ulong[]{0x0000000000000002UL});
    public static readonly BitSet FOLLOW_SUBIDENTIFIER_in_identifier181 = new BitSet(new ulong[]{0x0000000000004002UL});
    public static readonly BitSet FOLLOW_14_in_identifier184 = new BitSet(new ulong[]{0x0000000000000200UL});
    public static readonly BitSet FOLLOW_SUBIDENTIFIER_in_identifier185 = new BitSet(new ulong[]{0x0000000000004002UL});

}
}
