using System;
using AutoMapper;

namespace JobTimer.Utils
{
    public class CopyMapping
    {
        public Type Source { get; set; }
        public Type Target { get; set; }
        public CopyMapping(Type source, Type target)
        {
            Source = source;
            Target = target;
        }
    }

    public class Copier
    {
        protected void Copy(object source, object target)
        {
            if (source != null && target != null)
            {
                var sourceType = source.GetType();
                var targetType = target.GetType();

                Mapper.CreateMap(sourceType, targetType);
                Mapper.Map(source, target, sourceType, targetType);
            }
        }
        protected void Copy(object source, object target, params CopyMapping[] additionalMappings)
        {            
            if (source != null && target != null)
            {
                var sourceType = source.GetType();
                var targetType = target.GetType();

                Mapper.CreateMap(sourceType, targetType);

                if (additionalMappings != null)
                {
                    foreach (var mapp in additionalMappings)
                    {
                        Mapper.CreateMap(mapp.Source, mapp.Target);
                    }
                }

                Mapper.Map(source, target, sourceType, targetType);
            }
            else
            {
                throw new Exception("either source or target is null");
            }
        }
    }
}
