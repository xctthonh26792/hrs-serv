using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json.Linq;
using System.Linq;
using Tenjin.Helpers;

namespace Tenjin.Apis.Reflections.Models
{
    class HttpExpressionDefinition
    {
        public const string PRE_STAGE = "pre";
        public const string POST_STAGE = "post";
        private readonly JToken _token;
        private readonly string _name;
        private readonly string _category;
        private readonly string _stage;

        public HttpExpressionDefinition(JToken token)
        {
            _token = token;
            _name = token?.Value<string>("n");
            _category = token?.Value<string>("c");
            _stage = token?.Value<string>("s");
        }

        public bool IsCorrect
        {
            get
            {
                return _token != null
                    && TenjinUtils.IsStringNotEmpty(_name)
                    && TenjinUtils.IsStringNotEmpty(_category)
                    && TenjinUtils.IsStringNotEmpty(_stage)
                    && (_stage == PRE_STAGE || _stage == POST_STAGE);
            }
        }

        public string GetStage()
        {
            return _stage;
        }

        public FilterDefinition<T> GetDefinition<T>()
        {
            if (_category == "$in")
            {
                var value = _token.Value<JArray>("v");
                var elements = value?.Values<JValue>()?.Select(x => x.Value)?.Where(IsValuable)?.ToList();
                return TenjinUtils.IsArrayNotEmpty(elements) ? Builders<T>.Filter.In(_name, elements) : default;
            }
            else if (_category == ">")
            {
                var value = _token.Value<JValue>("v");
                var model = value?.Value;
                return IsValuable(model) ? Builders<T>.Filter.Gt(_name, model) : default;
            }
            else if (_category == ">=")
            {
                var value = _token.Value<JValue>("v");
                var model = value?.Value;
                return IsValuable(model) ? Builders<T>.Filter.Gte(_name, model) : default;
            }
            else if (_category == "<")
            {
                var value = _token.Value<JValue>("v");
                var model = value?.Value;
                return IsValuable(model) ? Builders<T>.Filter.Lt(_name, model) : default;
            }
            else if (_category == "<=")
            {
                var value = _token.Value<JValue>("v");
                var model = value?.Value;
                return IsValuable(model) ? Builders<T>.Filter.Lte(_name, model) : default;
            }
            else if (_category == "==")
            {
                var value = _token.Value<JValue>("v");
                var model = value?.Value;
                return IsValuable(model) ? Builders<T>.Filter.Eq(_name, model) : default;
            }
            else if (_category == "^")
            {
                var value = _token.Value<JValue>("v");
                var model = value?.Value;
                return IsValuable(model) ? Builders<T>.Filter.Regex(_name, new BsonRegularExpression("^" + model.ToString())) : default;
            }
            else if (_category == "$")
            {
                var value = _token.Value<JValue>("v");
                var model = value?.Value;
                return IsValuable(model) ? Builders<T>.Filter.Regex(_name, new BsonRegularExpression(model.ToString() + "$")) : default;
            }
            else if (_category == @"//")
            {
                var value = _token.Value<JValue>("v");
                var model = value?.Value;
                return IsValuable(model) ? Builders<T>.Filter.Regex(_name, new BsonRegularExpression(model.ToString())) : default;
            }
            else if (_category == "!=")
            {
                var value = _token.Value<JValue>("v");
                var model = value?.Value;
                return IsValuable(model) ? Builders<T>.Filter.Ne(_name, model) : default;
            }
            else if (_category == "[]")
            {
                var token = _token.Value<JToken>("v");
                var from = token?.Value<JValue>("f")?.Value;
                var to = token?.Value<JValue>("t")?.Value;
                if (IsValuable(from) && IsValuable(to))
                {
                    return Builders<T>.Filter.And(Builders<T>.Filter.Gte(_name, from), Builders<T>.Filter.Lte(_name, to));
                }
                if (IsValuable(from))
                {
                    return Builders<T>.Filter.Gte(_name, from);
                }
                if (IsValuable(to))
                {
                    return Builders<T>.Filter.Lte(_name, to);
                }
            }
            return default;
        }

        private static bool IsValuable(object value)
        {
            return value != null && TenjinUtils.IsStringNotEmpty(value.ToString());
        }
    }
}
