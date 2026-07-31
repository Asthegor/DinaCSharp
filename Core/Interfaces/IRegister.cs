using DinaCSharp.Services;

namespace DinaCSharp.Core.Interfaces
{

    public interface IRegister
    {
        public void Register(Key<ServiceTag> key);
    }
}
