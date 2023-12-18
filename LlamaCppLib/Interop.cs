﻿using System.Diagnostics;

namespace LlamaCppLib
{
    using llama_model = System.IntPtr;
    using llama_token = System.Int32;
    using llama_pos = System.Int32;
    using llama_seq_id = System.Int32;

    public static unsafe class Interop
    {
        public static void llama_batch_add(ref Native.llama_batch batch, llama_token id, llama_pos pos, llama_seq_id[] seq_ids, bool logits)
        {
            batch.token[batch.n_tokens] = id;
            batch.pos[batch.n_tokens] = pos;
            batch.n_seq_id[batch.n_tokens] = seq_ids.Length;

            for (var i = 0; i < seq_ids.Length; ++i)
                batch.seq_id[batch.n_tokens][i] = seq_ids[i];

            batch.logits[batch.n_tokens] = (sbyte)(logits ? 1 : 0);
            batch.n_tokens++;
        }

        public static Span<llama_token> llama_tokenize(llama_model model, string text, bool add_bos = false, bool special = false)
        {
            var n_tokens = text.Length + (add_bos ? 1 : 0);
            var result = new llama_token[n_tokens];
            fixed (llama_token* p1 = &result[0])
            {
                n_tokens = Native.llama_tokenize(model, text, text.Length, p1, result.Length, (byte)(add_bos ? 1 : 0), (byte)(special ? 1 : 0));
                if (n_tokens < 0)
                {
                    result = new llama_token[-n_tokens];
                    fixed (llama_token* p2 = &result[0])
                    {
                        var check = Native.llama_tokenize(model, text, text.Length, p2, result.Length, (byte)(add_bos ? 1 : 0), (byte)(special ? 1 : 0));
                        Debug.Assert(check == -n_tokens);
                        n_tokens = result.Length;
                    }
                }
            }

            return new(result, 0, n_tokens);
        }

        public static Span<byte> llama_token_to_piece(llama_model model, llama_token token)
        {
            var n_pieces = 0;
            var result = new byte[8];
            fixed (byte* p1 = &result[0])
            {
                n_pieces = Native.llama_token_to_piece(model, token, p1, result.Length);
                if (n_pieces < 0)
                {
                    result = new byte[-n_pieces];
                    fixed (byte* p2 = &result[0])
                    {
                        var check = Native.llama_token_to_piece(model, token, p2, result.Length);
                        Debug.Assert(check == -n_pieces);
                        n_pieces = result.Length;
                    }
                }

            }

            return new(result, 0, n_pieces);
        }
    }
}
