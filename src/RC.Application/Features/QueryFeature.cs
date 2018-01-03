﻿using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Rabbit.Cloud.Application.Features
{
    public class QueryFeature : IQueryFeature
    {
        private static readonly Func<IFeatureCollection, IRequestFeature> NullRequestFeature = f => null;

        private FeatureReferences<IRequestFeature> _features;

        private string _original;
        private IDictionary<string, StringValues> _parsedValues;

        private static readonly IDictionary<string, StringValues> Empty = new Dictionary<string, StringValues>();

        public QueryFeature(IDictionary<string, StringValues> query)
        {
            _parsedValues = query ?? throw new ArgumentNullException(nameof(query));
        }

        public QueryFeature(IFeatureCollection features)
        {
            if (features == null)
            {
                throw new ArgumentNullException(nameof(features));
            }

            _features = new FeatureReferences<IRequestFeature>(features);
        }

        private IRequestFeature RequestFeature =>
            _features.Fetch(ref _features.Cache, NullRequestFeature);

        #region Implementation of IQueryFeature

        public IDictionary<string, StringValues> Query
        {
            get
            {
                if (_features.Collection == null)
                {
                    return _parsedValues ?? (_parsedValues = Empty);
                }

                var current = RequestFeature.QueryString;
                if (_parsedValues != null && string.Equals(_original, current, StringComparison.Ordinal))
                    return _parsedValues;
                _original = current;

                var result = QueryHelpers.ParseNullableQuery(current);

                _parsedValues = result ?? Empty;
                return _parsedValues;
            }
            set
            {
                _parsedValues = value;
                if (_features.Collection == null) return;
                if (value == null)
                {
                    _original = string.Empty;
                    RequestFeature.QueryString = string.Empty;
                }
                else
                {
                    _original = QueryHelpers.AddQueryString(string.Empty, _parsedValues.ToDictionary(i => i.Key, i => i.Value.ToString()));
                    RequestFeature.QueryString = _original;
                }
            }
        }

        #endregion Implementation of IQueryFeature
    }
}