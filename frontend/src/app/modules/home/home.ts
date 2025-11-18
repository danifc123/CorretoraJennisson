import { Component, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './home.html',
  styleUrls: ['./home.scss']
})
export class Home implements OnInit, OnDestroy {
  private resizeListener: any;
  // Serviços/Categorias disponíveis
  services = [
    {
      icon: 'business',
      title: 'Loja Comercial',
      description: 'Imóveis comerciais',
      count: 120
    },
    {
      icon: 'home',
      title: 'Casas',
      description: 'Casas residenciais',
      count: 89
    },
    {
      icon: 'apartment',
      title: 'Apartamentos',
      description: 'Apartamentos modernos',
      count: 156
    },
    {
      icon: 'star',
      title: 'Destaques',
      description: 'Imóveis em destaque',
      count: 45
    },
    {
      icon: 'location_city',
      title: 'Prédios',
      description: 'Prédios comerciais',
      count: 23
    },
    {
      icon: 'shopping_cart',
      title: 'Comprar',
      description: 'Imóveis à venda',
      count: 234
    }
  ];

  // Controle do carrossel
  currentIndex = 0;
  itemsPerView = 3;

  ngOnInit() {
    this.updateItemsPerView();
    this.resizeListener = () => this.updateItemsPerView();
    window.addEventListener('resize', this.resizeListener);
  }

  ngOnDestroy() {
    if (this.resizeListener) {
      window.removeEventListener('resize', this.resizeListener);
    }
  }

  /**
   * Atualiza quantos items mostrar baseado na largura da tela
   */
  updateItemsPerView(): void {
    const width = window.innerWidth;
    if (width < 768) {
      this.itemsPerView = 1; // Mobile: 1 card
    } else if (width < 992) {
      this.itemsPerView = 2; // Tablet: 2 cards
    } else {
      this.itemsPerView = 3; // Desktop: 3 cards
    }

    // Reset index se estiver fora do alcance permitido
    const maxIndex = this.getMaxIndex();
    if (this.currentIndex > maxIndex) {
      this.currentIndex = maxIndex;
    }
  }

  // Touch/Swipe
  touchStartX = 0;
  touchEndX = 0;

  // Depoimentos de clientes
  testimonials = [
    {
      name: 'Maria Silva',
      role: 'Cliente Satisfeita',
      image: 'https://ui-avatars.com/api/?name=Maria+Silva&background=3498db&color=fff&size=200',
      text: 'Excelente atendimento! Encontrei o imóvel dos meus sonhos com a ajuda da equipe.'
    },
    {
      name: 'João Santos',
      role: 'Comprador',
      image: 'https://ui-avatars.com/api/?name=Joao+Santos&background=2c3e50&color=fff&size=200',
      text: 'Profissionais muito atenciosos e comprometidos. Recomendo!'
    },
    {
      name: 'Ana Costa',
      role: 'Investidora',
      image: 'https://ui-avatars.com/api/?name=Ana+Costa&background=e74c3c&color=fff&size=200',
      text: 'Ótima experiência! Processo rápido e transparente do início ao fim.'
    }
  ];

  /**
   * Navega para a página de imóveis com filtro
   */
  filterByCategory(category: string): void {
    console.log(`Filtrar por categoria: ${category}`);
    // TODO: Implementar navegação com filtro
  }

  /**
   * Busca por imóveis
   */
  searchProperties(event: Event): void {
    event.preventDefault();
    console.log('Buscar imóveis...');
    // TODO: Implementar busca
  }

  /**
   * Navega para o próximo item do carrossel
   */
  nextSlide(): void {
    const maxIndex = this.getMaxIndex();
    if (this.currentIndex < maxIndex) {
      this.currentIndex++;
    }
  }

  /**
   * Navega para o item anterior do carrossel
   */
  prevSlide(): void {
    if (this.currentIndex > 0) {
      this.currentIndex--;
    }
  }

  /**
   * Verifica se pode navegar para o próximo
   */
  canGoNext(): boolean {
    const maxIndex = Math.max(0, this.services.length - this.itemsPerView);
    return this.currentIndex < maxIndex;
  }

  /**
   * Verifica se pode navegar para o anterior
   */
  canGoPrev(): boolean {
    return this.currentIndex > 0;
  }

  /**
   * Retorna o índice máximo permitido
   */
  getMaxIndex(): number {
    return Math.max(0, this.services.length - this.itemsPerView);
  }

  /**
   * Retorna o estilo de transformação do carrossel
   */
  getCarouselTransform(): string {
    return `translateX(-${this.currentIndex * (100 / this.itemsPerView)}%)`;
  }

  /**
   * Inicia o toque/arrasto
   */
  onTouchStart(event: TouchEvent): void {
    this.touchStartX = event.changedTouches[0].screenX;
  }

  /**
   * Finaliza o toque/arrasto e detecta direção
   */
  onTouchEnd(event: TouchEvent): void {
    this.touchEndX = event.changedTouches[0].screenX;
    this.handleSwipe();
  }

  /**
   * Processa o swipe e navega
   */
  handleSwipe(): void {
    const swipeThreshold = 50; // Mínimo de 50px para considerar swipe
    const difference = this.touchStartX - this.touchEndX;

    if (Math.abs(difference) > swipeThreshold) {
      if (difference > 0) {
        // Swipe para esquerda = próximo
        this.nextSlide();
      } else {
        // Swipe para direita = anterior
        this.prevSlide();
      }
    }
  }
}

