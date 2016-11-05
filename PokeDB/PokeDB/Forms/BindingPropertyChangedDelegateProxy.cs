using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PokeDB.Forms
{
    abstract class BindingPropertyChangedDelegateProxyBase
    {
        public abstract void OnPropertyValueChanged(BindableObject bindable, object valueOld, object valueNew);

        public static implicit operator BindableProperty.BindingPropertyChangedDelegate(BindingPropertyChangedDelegateProxyBase proxy)
        {
            return proxy.OnPropertyValueChanged;
        }
    }

    class BindingPropertyChangedDelegateProxy<TOwner, TValue> : BindingPropertyChangedDelegateProxyBase
        where TOwner : BindableObject
        where TValue : class
    {
        readonly Action<TOwner, TValue, TValue> callback;

        public BindingPropertyChangedDelegateProxy(Action<TOwner, TValue, TValue> callback)
        {
#if DEBUG
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }
#endif // DEBUG
            this.callback = callback;
        }

        public override void OnPropertyValueChanged(BindableObject bindable, object valueOld, object valueNew)
        {
#if DEBUG
            var message = "{0} is of incompatible type:\n- Expected {1};\n- Found {2}.\n" 
                + $"Check your {nameof(BindableProperty)} definition!";

            if (!(bindable is TOwner))
            {
                throw new InvalidCastException(string.Format(message, nameof(bindable), 
                    typeof(TOwner).FullName, bindable.GetType().FullName));
            }
            if (valueOld != null && !(valueOld is TValue))
            {
                throw new InvalidCastException(string.Format(message, nameof(valueOld),
                    typeof(TValue).FullName, valueOld.GetType().FullName));
            }
            if (valueNew != null && !(valueNew is TValue))
            {
                throw new InvalidCastException(string.Format(message, nameof(valueNew),
                    typeof(TValue).FullName, valueNew.GetType().FullName));
            }
#endif //DEBUG
            callback((TOwner)bindable, (TValue)valueOld, (TValue)valueNew);
        }
    }
}
