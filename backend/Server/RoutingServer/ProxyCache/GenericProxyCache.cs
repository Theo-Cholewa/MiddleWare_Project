using System;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace ProxyCache
{
    public class GenericProxyCache<T> where T : class
    {
        private MemoryCache _cache = MemoryCache.Default; // Crée une instance de MemoryCache pour stocker les éléments en mémoire
        public DateTimeOffset dt_defaut = ObjectCache.InfiniteAbsoluteExpiration; // Définit un temps d'expiration par défaut (infini par défaut)

        // Méthode pour obtenir un élément du cache ou l'initialiser si non présent (avec expiration par défaut)
        public T Get(string nomElementCache, Func<Task<T>> initialiseur)
        {
            return Get(nomElementCache, dt_defaut, initialiseur); // Appelle une autre surcharge de la méthode Get avec l'expiration par défaut
        }

        // Méthode pour obtenir un élément du cache ou l'initialiser si non présent (avec expiration en secondes)
        public T Get(string nomElementCache, double secondesExpiration, Func<Task<T>> initialiseur)
        {
            return Get(nomElementCache, DateTimeOffset.Now.AddSeconds(secondesExpiration), initialiseur); // Convertit le temps en secondes en DateTimeOffset
        }

        // Méthode pour obtenir un élément du cache ou l'initialiser si non présent (avec expiration spécifiée)
        public T Get(string nomElementCache, DateTimeOffset dateExpiration, Func<Task<T>> initialiseur)
        {
            var element = _cache.Get(nomElementCache) as T; // Tente de récupérer l'élément du cache avec la clé donnée et le convertit en type T
            if (element == null)
            {
                Console.WriteLine("Élément non trouvé dans le cache. Initialisation et ajout au cache : " + nomElementCache);
                element = initialiseur().Result; // Initialise et met en cache le nouvel élément
                _cache.Set(nomElementCache, element, dateExpiration); // Ajoute le nouvel élément au cache avec le temps d'expiration spécifié
            }
            else
            {
                Console.WriteLine("Élément trouvé dans le cache : " + nomElementCache);
            }
            return element; // Retourne l'élément du cache (ou nouvellement initialisé)
        }
    }
}
