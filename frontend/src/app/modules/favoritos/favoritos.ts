import { Component, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

interface ImovelFavorito {
  id: number;
  titulo: string;
  tipo: string;
  localizacao: string;
  bairro: string;
  cidade: string;
  preco: number;
  quartos?: number;
  banheiros?: number;
  area?: number;
  vagas?: number;
  imagem?: string;
  descricao: string;
}

@Component({
  selector: 'app-favoritos',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './favoritos.html',
  styleUrl: './favoritos.scss'
})
export class Favoritos {
  // Estado de login (simulado - depois conectar com serviço de auth)
  isLoggedIn = signal(true); // TODO: Conectar com AuthService

  // Dados de exemplo (depois será substituído por chamada à API)
  favoritos = signal<ImovelFavorito[]>([
    {
      id: 1,
      titulo: 'Casa moderna com 3 quartos',
      tipo: 'Casa',
      localizacao: 'Manaíra',
      bairro: 'Manaíra',
      cidade: 'João Pessoa',
      preco: 450000,
      quartos: 3,
      banheiros: 2,
      area: 150,
      vagas: 2,
      descricao: 'Casa moderna em ótima localização, próximo à praia.',
      imagem: undefined
    },
    {
      id: 2,
      titulo: 'Apartamento 2 quartos',
      tipo: 'Apartamento',
      localizacao: 'Tambaú',
      bairro: 'Tambaú',
      cidade: 'João Pessoa',
      preco: 320000,
      quartos: 2,
      banheiros: 2,
      area: 80,
      vagas: 1,
      descricao: 'Apartamento bem localizado próximo ao centro comercial.',
      imagem: undefined
    }
  ]);

  /**
   * Formata preço para exibição
   */
  formatarPreco(preco: number): string {
    return new Intl.NumberFormat('pt-BR', {
      style: 'currency',
      currency: 'BRL',
      minimumFractionDigits: 0,
      maximumFractionDigits: 0
    }).format(preco);
  }

  /**
   * Remove imóvel dos favoritos
   */
  removerFavorito(imovel: ImovelFavorito): void {
    if (!confirm(`Tem certeza que deseja remover "${imovel.titulo}" dos seus favoritos?`)) {
      return;
    }

    // TODO: Implementar chamada à API
    this.favoritos.set(this.favoritos().filter(f => f.id !== imovel.id));
  }

  /**
   * Limpa todos os favoritos
   */
  limparFavoritos(): void {
    if (!confirm('Tem certeza que deseja remover todos os imóveis dos seus favoritos?')) {
      return;
    }

    // TODO: Implementar chamada à API
    this.favoritos.set([]);
  }
}

