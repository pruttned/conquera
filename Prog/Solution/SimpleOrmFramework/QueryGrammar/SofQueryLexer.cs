//////////////////////////////////////////////////////////////////////
//  Copyright (C) 2010 by Conquera Team
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

// $ANTLR 3.1.2 D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g 2009-12-23 12:47:49

// The variable 'variable' is assigned but its value is never used.
#pragma warning disable 168, 219
// Unreachable code detected.
#pragma warning disable 162


using System;
using Antlr.Runtime;
using IList 		= System.Collections.IList;
using ArrayList 	= System.Collections.ArrayList;
using Stack 		= Antlr.Runtime.Collections.StackList;


namespace  SimpleOrmFramework 
{
internal partial class SofQueryLexer : Lexer {
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

    public SofQueryLexer() 
    {
		InitializeCyclicDFAs();
    }
    public SofQueryLexer(ICharStream input)
		: this(input, null) {
    }
    public SofQueryLexer(ICharStream input, RecognizerSharedState state)
		: base(input, state) {
		InitializeCyclicDFAs(); 

    }
    
    override public string GrammarFileName
    {
    	get { return "D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g";} 
    }

    // $ANTLR start "T__12"
    public void mT__12() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__12;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:9:9: ( '(' )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:9:9: '('
            {
            	Match('('); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__12"

    // $ANTLR start "T__13"
    public void mT__13() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__13;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:10:9: ( ')' )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:10:9: ')'
            {
            	Match(')'); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__13"

    // $ANTLR start "T__14"
    public void mT__14() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__14;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:11:9: ( '.' )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:11:9: '.'
            {
            	Match('.'); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__14"

    // $ANTLR start "T__15"
    public void mT__15() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__15;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:12:9: ( '<' )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:12:9: '<'
            {
            	Match('<'); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__15"

    // $ANTLR start "T__16"
    public void mT__16() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__16;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:13:9: ( '<=' )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:13:9: '<='
            {
            	Match("<="); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__16"

    // $ANTLR start "T__17"
    public void mT__17() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__17;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:14:9: ( '<>' )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:14:9: '<>'
            {
            	Match("<>"); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__17"

    // $ANTLR start "T__18"
    public void mT__18() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__18;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:15:9: ( '=' )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:15:9: '='
            {
            	Match('='); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__18"

    // $ANTLR start "T__19"
    public void mT__19() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__19;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:16:9: ( '>' )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:16:9: '>'
            {
            	Match('>'); 

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__19"

    // $ANTLR start "T__20"
    public void mT__20() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = T__20;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:17:9: ( '>=' )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:17:9: '>='
            {
            	Match(">="); 


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "T__20"

    // $ANTLR start "AND"
    public void mAND() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = AND;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:42:9: ( ( 'a' | 'A' ) ( 'n' | 'N' ) ( 'd' | 'D' ) )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:42:9: ( 'a' | 'A' ) ( 'n' | 'N' ) ( 'd' | 'D' )
            {
            	if ( input.LA(1) == 'A' || input.LA(1) == 'a' ) 
            	{
            	    input.Consume();

            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}

            	if ( input.LA(1) == 'N' || input.LA(1) == 'n' ) 
            	{
            	    input.Consume();

            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}

            	if ( input.LA(1) == 'D' || input.LA(1) == 'd' ) 
            	{
            	    input.Consume();

            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "AND"

    // $ANTLR start "OR"
    public void mOR() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = OR;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:44:8: ( ( 'o' | 'O' ) ( 'r' | 'R' ) )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:44:8: ( 'o' | 'O' ) ( 'r' | 'R' )
            {
            	if ( input.LA(1) == 'O' || input.LA(1) == 'o' ) 
            	{
            	    input.Consume();

            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}

            	if ( input.LA(1) == 'R' || input.LA(1) == 'r' ) 
            	{
            	    input.Consume();

            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "OR"

    // $ANTLR start "SUBIDENTIFIER"
    public void mSUBIDENTIFIER() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = SUBIDENTIFIER;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:47:17: ( LETTER ( LETTER | '0' .. '9' )* )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:47:17: LETTER ( LETTER | '0' .. '9' )*
            {
            	mLETTER(); 
            	// D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:47:24: ( LETTER | '0' .. '9' )*
            	do 
            	{
            	    int alt1 = 2;
            	    int LA1_0 = input.LA(1);

            	    if ( ((LA1_0 >= '0' && LA1_0 <= '9') || (LA1_0 >= 'A' && LA1_0 <= 'Z') || LA1_0 == '_' || (LA1_0 >= 'a' && LA1_0 <= 'z')) )
            	    {
            	        alt1 = 1;
            	    }


            	    switch (alt1) 
            		{
            			case 1 :
            			    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:
            			    {
            			    	if ( (input.LA(1) >= '0' && input.LA(1) <= '9') || (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
            			    	{
            			    	    input.Consume();

            			    	}
            			    	else 
            			    	{
            			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
            			    	    Recover(mse);
            			    	    throw mse;}


            			    }
            			    break;

            			default:
            			    goto loop1;
            	    }
            	} while (true);

            	loop1:
            		;	// Stops C# compiler whining that label 'loop1' has no statements


            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "SUBIDENTIFIER"

    // $ANTLR start "VALUE"
    public void mVALUE() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = VALUE;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:50:10: ( ( '0' .. '9' )+ ( '.' ( '0' .. '9' )+ )? | '\\'' ( options {greedy=false; } : . )* '\\'' )
            int alt6 = 2;
            int LA6_0 = input.LA(1);

            if ( ((LA6_0 >= '0' && LA6_0 <= '9')) )
            {
                alt6 = 1;
            }
            else if ( (LA6_0 == '\'') )
            {
                alt6 = 2;
            }
            else 
            {
                NoViableAltException nvae_d6s0 =
                    new NoViableAltException("", 6, 0, input);

                throw nvae_d6s0;
            }
            switch (alt6) 
            {
                case 1 :
                    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:50:10: ( '0' .. '9' )+ ( '.' ( '0' .. '9' )+ )?
                    {
                    	// D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:50:10: ( '0' .. '9' )+
                    	int cnt2 = 0;
                    	do 
                    	{
                    	    int alt2 = 2;
                    	    int LA2_0 = input.LA(1);

                    	    if ( ((LA2_0 >= '0' && LA2_0 <= '9')) )
                    	    {
                    	        alt2 = 1;
                    	    }


                    	    switch (alt2) 
                    		{
                    			case 1 :
                    			    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:
                    			    {
                    			    	if ( (input.LA(1) >= '0' && input.LA(1) <= '9') ) 
                    			    	{
                    			    	    input.Consume();

                    			    	}
                    			    	else 
                    			    	{
                    			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    			    	    Recover(mse);
                    			    	    throw mse;}


                    			    }
                    			    break;

                    			default:
                    			    if ( cnt2 >= 1 ) goto loop2;
                    		            EarlyExitException eee2 =
                    		                new EarlyExitException(2, input);
                    		            throw eee2;
                    	    }
                    	    cnt2++;
                    	} while (true);

                    	loop2:
                    		;	// Stops C# compiler whining that label 'loop2' has no statements

                    	// D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:50:21: ( '.' ( '0' .. '9' )+ )?
                    	int alt4 = 2;
                    	int LA4_0 = input.LA(1);

                    	if ( (LA4_0 == '.') )
                    	{
                    	    alt4 = 1;
                    	}
                    	switch (alt4) 
                    	{
                    	    case 1 :
                    	        // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:50:22: '.' ( '0' .. '9' )+
                    	        {
                    	        	Match('.'); 
                    	        	// D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:50:25: ( '0' .. '9' )+
                    	        	int cnt3 = 0;
                    	        	do 
                    	        	{
                    	        	    int alt3 = 2;
                    	        	    int LA3_0 = input.LA(1);

                    	        	    if ( ((LA3_0 >= '0' && LA3_0 <= '9')) )
                    	        	    {
                    	        	        alt3 = 1;
                    	        	    }


                    	        	    switch (alt3) 
                    	        		{
                    	        			case 1 :
                    	        			    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:
                    	        			    {
                    	        			    	if ( (input.LA(1) >= '0' && input.LA(1) <= '9') ) 
                    	        			    	{
                    	        			    	    input.Consume();

                    	        			    	}
                    	        			    	else 
                    	        			    	{
                    	        			    	    MismatchedSetException mse = new MismatchedSetException(null,input);
                    	        			    	    Recover(mse);
                    	        			    	    throw mse;}


                    	        			    }
                    	        			    break;

                    	        			default:
                    	        			    if ( cnt3 >= 1 ) goto loop3;
                    	        		            EarlyExitException eee3 =
                    	        		                new EarlyExitException(3, input);
                    	        		            throw eee3;
                    	        	    }
                    	        	    cnt3++;
                    	        	} while (true);

                    	        	loop3:
                    	        		;	// Stops C# compiler whining that label 'loop3' has no statements


                    	        }
                    	        break;

                    	}


                    }
                    break;
                case 2 :
                    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:51:4: '\\'' ( options {greedy=false; } : . )* '\\''
                    {
                    	Match('\''); 
                    	// D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:51:8: ( options {greedy=false; } : . )*
                    	do 
                    	{
                    	    int alt5 = 2;
                    	    int LA5_0 = input.LA(1);

                    	    if ( (LA5_0 == '\'') )
                    	    {
                    	        alt5 = 2;
                    	    }
                    	    else if ( ((LA5_0 >= '\u0000' && LA5_0 <= '&') || (LA5_0 >= '(' && LA5_0 <= '\uFFFF')) )
                    	    {
                    	        alt5 = 1;
                    	    }


                    	    switch (alt5) 
                    		{
                    			case 1 :
                    			    // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:51:36: .
                    			    {
                    			    	MatchAny(); 

                    			    }
                    			    break;

                    			default:
                    			    goto loop5;
                    	    }
                    	} while (true);

                    	loop5:
                    		;	// Stops C# compiler whining that label 'loop5' has no statements

                    	Match('\''); 

                    }
                    break;

            }
            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "VALUE"

    // $ANTLR start "LETTER"
    public void mLETTER() // throws RecognitionException [2]
    {
    		try
    		{
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:56:11: ( 'A' .. 'Z' | 'a' .. 'z' | '_' )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:
            {
            	if ( (input.LA(1) >= 'A' && input.LA(1) <= 'Z') || input.LA(1) == '_' || (input.LA(1) >= 'a' && input.LA(1) <= 'z') ) 
            	{
            	    input.Consume();

            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}


            }

        }
        finally 
    	{
        }
    }
    // $ANTLR end "LETTER"

    // $ANTLR start "WS"
    public void mWS() // throws RecognitionException [2]
    {
    		try
    		{
            int _type = WS;
    	int _channel = DEFAULT_TOKEN_CHANNEL;
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:60:8: ( ( ' ' | '\\r' | '\\t' | '\\u000C' | '\\n' ) )
            // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:60:8: ( ' ' | '\\r' | '\\t' | '\\u000C' | '\\n' )
            {
            	if ( (input.LA(1) >= '\t' && input.LA(1) <= '\n') || (input.LA(1) >= '\f' && input.LA(1) <= '\r') || input.LA(1) == ' ' ) 
            	{
            	    input.Consume();

            	}
            	else 
            	{
            	    MismatchedSetException mse = new MismatchedSetException(null,input);
            	    Recover(mse);
            	    throw mse;}

            	_channel=HIDDEN;

            }

            state.type = _type;
            state.channel = _channel;
        }
        finally 
    	{
        }
    }
    // $ANTLR end "WS"

    override public void mTokens() // throws RecognitionException 
    {
        // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:10: ( T__12 | T__13 | T__14 | T__15 | T__16 | T__17 | T__18 | T__19 | T__20 | AND | OR | SUBIDENTIFIER | VALUE | WS )
        int alt7 = 14;
        alt7 = dfa7.Predict(input);
        switch (alt7) 
        {
            case 1 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:10: T__12
                {
                	mT__12(); 

                }
                break;
            case 2 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:16: T__13
                {
                	mT__13(); 

                }
                break;
            case 3 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:22: T__14
                {
                	mT__14(); 

                }
                break;
            case 4 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:28: T__15
                {
                	mT__15(); 

                }
                break;
            case 5 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:34: T__16
                {
                	mT__16(); 

                }
                break;
            case 6 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:40: T__17
                {
                	mT__17(); 

                }
                break;
            case 7 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:46: T__18
                {
                	mT__18(); 

                }
                break;
            case 8 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:52: T__19
                {
                	mT__19(); 

                }
                break;
            case 9 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:58: T__20
                {
                	mT__20(); 

                }
                break;
            case 10 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:64: AND
                {
                	mAND(); 

                }
                break;
            case 11 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:68: OR
                {
                	mOR(); 

                }
                break;
            case 12 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:71: SUBIDENTIFIER
                {
                	mSUBIDENTIFIER(); 

                }
                break;
            case 13 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:85: VALUE
                {
                	mVALUE(); 

                }
                break;
            case 14 :
                // D:\\Prog\\Projects\\AL\\Prog\\Solution\\SimpleOrmFramework\\QueryGrammar\\SofQuery.g:1:91: WS
                {
                	mWS(); 

                }
                break;

        }

    }


    protected DFA7 dfa7;
	private void InitializeCyclicDFAs()
	{
	    this.dfa7 = new DFA7(this);
	}

    const string DFA7_eotS =
        "\x4\xFFFF\x1\xE\x1\xFFFF\x1\x10\x2\x9\x8\xFFFF\x1\x9\x1\x14\x1\x15"+
        "\x2\xFFFF";
    const string DFA7_eofS =
        "\x16\xFFFF";
    const string DFA7_minS =
        "\x1\x9\x3\xFFFF\x1\x3D\x1\xFFFF\x1\x3D\x1\x4E\x1\x52\x8\xFFFF\x1"+
        "\x44\x2\x30\x2\xFFFF";
    const string DFA7_maxS =
        "\x1\x7A\x3\xFFFF\x1\x3E\x1\xFFFF\x1\x3D\x1\x6E\x1\x72\x8\xFFFF\x1"+
        "\x64\x2\x7A\x2\xFFFF";
    const string DFA7_acceptS =
        "\x1\xFFFF\x1\x1\x1\x2\x1\x3\x1\xFFFF\x1\x7\x3\xFFFF\x1\xC\x1\xD"+
        "\x1\xE\x1\x5\x1\x6\x1\x4\x1\x9\x1\x8\x3\xFFFF\x1\xB\x1\xA";
    const string DFA7_specialS =
        "\x16\xFFFF}>";
    static readonly string[] DFA7_transitionS = {
            "\x2\xB\x1\xFFFF\x2\xB\x12\xFFFF\x1\xB\x6\xFFFF\x1\xA\x1\x1\x1"+
            "\x2\x4\xFFFF\x1\x3\x1\xFFFF\xA\xA\x2\xFFFF\x1\x4\x1\x5\x1\x6"+
            "\x2\xFFFF\x1\x7\xD\x9\x1\x8\xB\x9\x4\xFFFF\x1\x9\x1\xFFFF\x1"+
            "\x7\xD\x9\x1\x8\xB\x9",
            "",
            "",
            "",
            "\x1\xC\x1\xD",
            "",
            "\x1\xF",
            "\x1\x11\x1F\xFFFF\x1\x11",
            "\x1\x12\x1F\xFFFF\x1\x12",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "",
            "\x1\x13\x1F\xFFFF\x1\x13",
            "\xA\x9\x7\xFFFF\x1A\x9\x4\xFFFF\x1\x9\x1\xFFFF\x1A\x9",
            "\xA\x9\x7\xFFFF\x1A\x9\x4\xFFFF\x1\x9\x1\xFFFF\x1A\x9",
            "",
            ""
    };

    static readonly short[] DFA7_eot = DFA.UnpackEncodedString(DFA7_eotS);
    static readonly short[] DFA7_eof = DFA.UnpackEncodedString(DFA7_eofS);
    static readonly char[] DFA7_min = DFA.UnpackEncodedStringToUnsignedChars(DFA7_minS);
    static readonly char[] DFA7_max = DFA.UnpackEncodedStringToUnsignedChars(DFA7_maxS);
    static readonly short[] DFA7_accept = DFA.UnpackEncodedString(DFA7_acceptS);
    static readonly short[] DFA7_special = DFA.UnpackEncodedString(DFA7_specialS);
    static readonly short[][] DFA7_transition = DFA.UnpackEncodedStringArray(DFA7_transitionS);

    protected class DFA7 : DFA
    {
        public DFA7(BaseRecognizer recognizer)
        {
            this.recognizer = recognizer;
            this.decisionNumber = 7;
            this.eot = DFA7_eot;
            this.eof = DFA7_eof;
            this.min = DFA7_min;
            this.max = DFA7_max;
            this.accept = DFA7_accept;
            this.special = DFA7_special;
            this.transition = DFA7_transition;

        }

        override public string Description
        {
            get { return "1:0: Tokens : ( T__12 | T__13 | T__14 | T__15 | T__16 | T__17 | T__18 | T__19 | T__20 | AND | OR | SUBIDENTIFIER | VALUE | WS );"; }
        }

    }

 
    
}
}
