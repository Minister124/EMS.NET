using Blazored.LocalStorage;

namespace ClientLibrary.Helper
{
    public class LocalStorageService(ILocalStorageService localStorageService)
    {
        private const string storageKey = "authentication-token";
        public async Task<string> GetToken()
        {
            return await localStorageService.GetItemAsStringAsync(storageKey);
        }
        public async Task SetToken(string Item)
        {
            await localStorageService.SetItemAsStringAsync(storageKey, Item);
        }

        public async Task RemoveToken()
        {
            await localStorageService.RemoveItemAsync(storageKey);
        }
    }
}
