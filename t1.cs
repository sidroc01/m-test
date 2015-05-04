    public class t1
    {
        public void M()
        {
            dynamic context = null;
            context.Request.Form["IntProperty_00"] = "1";
            context.Request.Form["IntProperty_01"] = "2";
            context.Request.Form["IntProperty_02"] = "3";
            context.Request.Form["IntProperty_03"] = "4";
            context.Request.Form["IntProperty_04"] = "5";
            context.Request.Form["IntProperty_05"] = "6";
            context.Request.Form["IntProperty_06"] = "7";
            context.Request.Form["IntProperty_07"] = "8";
            context.Request.Form["IntProperty_08"] = "9";
            context.Request.Form["IntProperty_09"] = "10";
            context.Request.Form["IntProperty_10"] = "11";
            context.Request.Form["IntProperty_11"] = "12";
        }
		
		public TypeMap FindTypeMap(object source, object destination, Type sourceType, Type destinationType, string profileName)
		{
			TypeMap typeMap = FindExplicitlyDefinedTypeMap(sourceType, destinationType);

			if (typeMap == null && destinationType.IsNullableType())
			{
				typeMap = FindExplicitlyDefinedTypeMap(sourceType, destinationType.GetTypeOfNullable());
			}

			if (typeMap == null)
			{
				typeMap = _typeMaps.FirstOrDefault(x => x.SourceType == sourceType && x.GetDerivedTypeFor(sourceType) == destinationType);

				if (typeMap == null)
				{
					foreach (var sourceInterface in sourceType.GetInterfaces())
					{
						typeMap = ((IConfigurationProvider)this).FindTypeMapFor(source, destination, sourceInterface, destinationType);

						if (typeMap == null) continue;

						var derivedTypeFor = typeMap.GetDerivedTypeFor(sourceType);
						if (derivedTypeFor != destinationType)
						{
							typeMap = CreateTypeMap(sourceType, derivedTypeFor, profileName, typeMap.ConfiguredMemberList);
						}

						break;
					}

					if ((sourceType.BaseType() != null) && (typeMap == null))
						typeMap = ((IConfigurationProvider)this).FindTypeMapFor(source, destination, sourceType.BaseType(), destinationType);
				}
			}
			if (typeMap == null && sourceType.IsGenericType() && destinationType.IsGenericType())
			{
				var sourceGenericDefinition = sourceType.GetGenericTypeDefinition();
				var destGenericDefinition = destinationType.GetGenericTypeDefinition();
				if (sourceGenericDefinition == null || destGenericDefinition == null)
				{
					return null;
				}
				var genericTypePair = new TypePair(sourceGenericDefinition, destGenericDefinition);
				CreateTypeMapExpression genericTypeMapExpression;

				if (_typeMapExpressionCache.TryGetValue(genericTypePair, out genericTypeMapExpression))
				{
					typeMap = CreateTypeMap(sourceType, destinationType, genericTypeMapExpression.ProfileName,
						genericTypeMapExpression.MemberList);

					var mappingExpression = CreateMappingExpression(typeMap, destinationType);

					genericTypeMapExpression.Accept(mappingExpression);
				}
			}
			return typeMap;
		}		
    }
