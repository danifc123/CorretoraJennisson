/**
 * Configuração da API
 * Detecta automaticamente se está em produção ou desenvolvimento
 * Usa getters para garantir que a detecção acontece em runtime, não em build-time
 */

// URLs de produção e desenvolvimento
const PRODUCTION_API_URL = 'https://corretorajennisson-backend.onrender.com/api';
const DEVELOPMENT_API_URL = 'http://localhost:5166/api';
const PRODUCTION_CHAT_HUB_URL = 'https://corretorajennisson-backend.onrender.com/chathub';
const DEVELOPMENT_CHAT_HUB_URL = 'http://localhost:5166/chathub';

// Função que detecta se está em produção (executada em runtime)
function isProduction(): boolean {
  if (typeof window === 'undefined') return false;

  const hostname = window.location.hostname.toLowerCase();
  const isLocalhost = hostname === 'localhost' ||
                      hostname === '127.0.0.1' ||
                      hostname.includes('localhost');

  return !isLocalhost;
}

// Getter para baseUrl (executado toda vez que é acessado)
function getApiBaseUrl(): string {
  const url = isProduction() ? PRODUCTION_API_URL : DEVELOPMENT_API_URL;

  // Log apenas na primeira vez para não poluir o console
  if (!(window as any).__API_CONFIG_LOGGED) {
    console.log('[API Config] Hostname:', window.location.hostname);
    console.log('[API Config] Is Production:', isProduction());
    console.log('[API Config] API Base URL:', url);
    (window as any).__API_CONFIG_LOGGED = true;
  }

  return url;
}

// Getter para chatHub (executado toda vez que é acessado)
function getChatHubUrl(): string {
  return isProduction() ? PRODUCTION_CHAT_HUB_URL : DEVELOPMENT_CHAT_HUB_URL;
}

export const API_CONFIG = {
  // Usa getter para garantir execução em runtime
  get baseUrl() {
    return getApiBaseUrl();
  },
  endpoints: {
    auth: {
      loginAdmin: '/auth/login-administrador',
      loginUsuario: '/auth/login-usuario',
      refreshToken: '/auth/refresh-token',
      identifyUserType: '/auth/identify-user-type',
      forgotPassword: '/auth/forgot-password',
      resetPassword: '/auth/reset-password'
    },
    imoveis: '/imovel',
    favoritos: '/favorito',
    imagens: '/imagemimovel',
    usuarios: '/usuario',
    administradores: '/administrador',
    conteudoSite: '/conteudosite',
    mensagens: '/mensagem',
    // Usa getter para chatHub também
    get chatHub() {
      return getChatHubUrl();
    }
  }
};

/**
 * Chaves para localStorage
 */
export const STORAGE_KEYS = {
  accessToken: 'access_token',
  refreshToken: 'refresh_token',
  user: 'user_data',
  role: 'user_role'
};

