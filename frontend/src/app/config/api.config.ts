/**
 * Configuração da API
 * Detecta automaticamente se está em produção ou desenvolvimento
 */
const isProduction = window.location.hostname !== 'localhost' && window.location.hostname !== '127.0.0.1';
const API_BASE_URL = isProduction
  ? 'https://corretorajennisson-backend.onrender.com/api'
  : 'http://localhost:5166/api';

const CHAT_HUB_URL = isProduction
  ? 'https://corretorajennisson-backend.onrender.com/chathub'
  : 'http://localhost:5166/chathub';

export const API_CONFIG = {
  baseUrl: API_BASE_URL,
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
    chatHub: CHAT_HUB_URL
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

