import { Component, signal, computed, ViewChild, ElementRef, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { forkJoin } from 'rxjs';
import { ImovelService, Imovel, StatusImovel, TipoImovel, CreateImovelRequest } from '../../../services/imovel.service';
import { ImagemImovelService } from '../../../services/imagem-imovel.service';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-imoveis-admin',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './imoveis-admin.html',
  styleUrls: ['./imoveis-admin.scss']
})
export class ImoveisAdmin implements OnInit {
  @ViewChild('fileInput') fileInput?: ElementRef<HTMLInputElement>;

  // Estados
  loading = signal(false);
  errorMessage = signal('');
  successMessage = signal('');
  showForm = signal(false);
  editingImovel = signal<Imovel | null>(null);
  uploadingImages = signal(false);

  // Formulário
  formData: Partial<CreateImovelRequest> = {
    titulo: '',
    tipoImovel: TipoImovel.Casa,
    estado: 'PB',
    cidade: 'João Pessoa',
    endereco: '',
    preco: 0,
    status: StatusImovel.Disponivel,
    descricao: ''
  };

  imagensSelecionadas = signal<File[]>([]);
  imagensPreview = signal<string[]>([]);
  imagensExistentes = signal<string[]>([]); // URLs das imagens já salvas

  // Filtros e busca
  searchTerm = signal('');
  tipoFiltro = signal<TipoImovel | ''>('');
  statusFiltro = signal<StatusImovel | ''>('');

  // Dados carregados da API
  imoveis = signal<Imovel[]>([]);

  // Opções
  tipos = Object.values(TipoImovel);
  statusOptions = Object.values(StatusImovel);
  estados = ['PB', 'PE', 'RN', 'CE', 'AL', 'SE', 'BA'];
  cidades = signal<string[]>([]);

  // Estado de autenticação
  isAdmin = computed(() => this.authService.isAdmin());

  constructor(
    private imovelService: ImovelService,
    private imagemImovelService: ImagemImovelService,
    private authService: AuthService,
    private router: Router
  ) {}

  ngOnInit(): void {
    // Verifica se é admin
    if (!this.isAdmin()) {
      this.router.navigate(['/']);
      return;
    }

    this.carregarImoveis();
  }

  /**
   * Carrega todos os imóveis da API
   */
  carregarImoveis(): void {
    this.loading.set(true);
    this.errorMessage.set('');

    this.imovelService.getAll().subscribe({
      next: (imoveis) => {
        this.imoveis.set(imoveis);
        this.extrairCidades(imoveis);
        this.loading.set(false);
      },
      error: (error) => {
        console.error('Erro ao carregar imóveis:', error);
        this.errorMessage.set('Erro ao carregar imóveis. Tente novamente mais tarde.');
        this.loading.set(false);
      }
    });
  }

  /**
   * Extrai lista única de cidades dos imóveis
   */
  private extrairCidades(imoveis: Imovel[]): void {
    const cidadesSet = new Set<string>();
    imoveis.forEach(imovel => {
      if (imovel.cidade) cidadesSet.add(imovel.cidade);
    });
    this.cidades.set(Array.from(cidadesSet).sort());
  }

  /**
   * Imóveis filtrados
   */
  imoveisFiltrados = computed(() => {
    let resultado = [...this.imoveis()];

    const busca = this.searchTerm().toLowerCase();
    if (busca) {
      resultado = resultado.filter(imovel =>
        imovel.titulo?.toLowerCase().includes(busca) ||
        imovel.endereco?.toLowerCase().includes(busca) ||
        imovel.cidade?.toLowerCase().includes(busca)
      );
    }

    if (this.tipoFiltro()) {
      resultado = resultado.filter(imovel => imovel.tipoImovel === this.tipoFiltro());
    }

    if (this.statusFiltro()) {
      resultado = resultado.filter(imovel => imovel.status === this.statusFiltro());
    }

    return resultado;
  });

  /**
   * Formata preço
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
   * Obtém URL da primeira imagem do imóvel
   */
  getImagemUrl(imovel: Imovel): string {
    if (imovel.imagens && imovel.imagens.length > 0) {
      return imovel.imagens[0].url;
    }
    return '/images/placeholder-imovel.jpg';
  }

  /**
   * Abre formulário para novo imóvel
   */
  novoImovel(): void {
    this.editingImovel.set(null);
    this.formData = {
      titulo: '',
      tipoImovel: TipoImovel.Casa,
      estado: 'PB',
      cidade: 'João Pessoa',
      endereco: '',
      preco: 0,
      status: StatusImovel.Disponivel,
      descricao: ''
    };
    this.imagensSelecionadas.set([]);
    this.imagensPreview.set([]);
    this.imagensExistentes.set([]);
    this.errorMessage.set('');
    this.successMessage.set('');
    this.showForm.set(true);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  /**
   * Abre formulário para editar imóvel
   */
  editarImovel(imovel: Imovel): void {
    this.editingImovel.set(imovel);
    this.formData = {
      titulo: imovel.titulo,
      tipoImovel: imovel.tipoImovel,
      estado: imovel.estado,
      cidade: imovel.cidade,
      endereco: imovel.endereco,
      preco: imovel.preco,
      status: imovel.status,
      descricao: imovel.descricao
    };
    this.imagensSelecionadas.set([]);
    this.imagensPreview.set([]);

    // Carrega imagens existentes do imóvel
    if (imovel.imagens && imovel.imagens.length > 0) {
      this.imagensExistentes.set(imovel.imagens.map(img => img.url));
    } else {
      this.imagensExistentes.set([]);
    }

    this.errorMessage.set('');
    this.successMessage.set('');
    this.showForm.set(true);
    window.scrollTo({ top: 0, behavior: 'smooth' });
  }

  /**
   * Cancela edição/criação
   */
  cancelar(): void {
    this.showForm.set(false);
    this.editingImovel.set(null);
    this.formData = {};
    this.imagensSelecionadas.set([]);
    this.imagensPreview.set([]);
    this.imagensExistentes.set([]);
    this.errorMessage.set('');
    this.successMessage.set('');
  }

  /**
   * Salva imóvel (criar ou editar)
   */
  salvarImovel(): void {
    // Validações
    if (!this.formData.titulo || !this.formData.tipoImovel || !this.formData.cidade ||
        !this.formData.endereco || !this.formData.preco || !this.formData.descricao ||
        !this.formData.estado || !this.formData.status) {
      this.errorMessage.set('Por favor, preencha todos os campos obrigatórios.');
      return;
    }

    if (this.formData.preco! <= 0) {
      this.errorMessage.set('O preço deve ser maior que zero.');
      return;
    }

    this.loading.set(true);
    this.errorMessage.set('');

    const imovelData: CreateImovelRequest = {
      titulo: this.formData.titulo!,
      tipoImovel: this.formData.tipoImovel!,
      estado: this.formData.estado!,
      cidade: this.formData.cidade!,
      endereco: this.formData.endereco!,
      preco: this.formData.preco!,
      status: this.formData.status!,
      descricao: this.formData.descricao!
    };

    const editing = this.editingImovel();

    if (editing) {
      // Editar imóvel existente
      this.imovelService.update(editing.id, imovelData).subscribe({
        next: (imovelAtualizado) => {
          // Faz upload das novas imagens se houver
          const novasImagens = this.imagensSelecionadas();
          if (novasImagens.length > 0) {
            this.uploadImagens(imovelAtualizado.id, novasImagens, () => {
              this.loading.set(false);
              this.successMessage.set('Imóvel atualizado com sucesso!');
              this.cancelar();
              this.carregarImoveis();
              setTimeout(() => this.successMessage.set(''), 3000);
            });
          } else {
            this.loading.set(false);
            this.successMessage.set('Imóvel atualizado com sucesso!');
            this.cancelar();
            this.carregarImoveis();
            setTimeout(() => this.successMessage.set(''), 3000);
          }
        },
        error: (error) => {
          console.error('Erro ao atualizar imóvel:', error);
          this.errorMessage.set('Erro ao atualizar imóvel. Tente novamente.');
          this.loading.set(false);
        }
      });
    } else {
      // Criar novo imóvel
      this.imovelService.create(imovelData).subscribe({
        next: (novoImovel) => {
          // Faz upload das imagens
          const imagens = this.imagensSelecionadas();
          if (imagens.length > 0) {
            this.uploadImagens(novoImovel.id, imagens, () => {
              this.loading.set(false);
              this.successMessage.set('Imóvel criado com sucesso!');
              this.cancelar();
              this.carregarImoveis();
              setTimeout(() => this.successMessage.set(''), 3000);
            });
          } else {
            this.loading.set(false);
            this.successMessage.set('Imóvel criado com sucesso!');
            this.cancelar();
            this.carregarImoveis();
            setTimeout(() => this.successMessage.set(''), 3000);
          }
        },
        error: (error) => {
          console.error('Erro ao criar imóvel:', error);
          this.errorMessage.set('Erro ao criar imóvel. Tente novamente.');
          this.loading.set(false);
        }
      });
    }
  }

  /**
   * Faz upload de múltiplas imagens
   */
  private uploadImagens(imovelId: number, files: File[], callback: () => void): void {
    this.uploadingImages.set(true);

    const uploads = files.map(file =>
      this.imagemImovelService.upload(imovelId, file)
    );

    forkJoin(uploads).subscribe({
      next: () => {
        this.uploadingImages.set(false);
        callback();
      },
      error: (error) => {
        console.error('Erro ao fazer upload de imagens:', error);
        this.uploadingImages.set(false);
        this.errorMessage.set('Erro ao fazer upload de algumas imagens. O imóvel foi salvo, mas algumas imagens podem não ter sido enviadas.');
        callback();
      }
    });
  }

  /**
   * Deleta imóvel
   */
  deletarImovel(imovel: Imovel): void {
    if (!confirm(`Tem certeza que deseja excluir o imóvel "${imovel.titulo}"? Esta ação não pode ser desfeita.`)) {
      return;
    }

    this.loading.set(true);

    this.imovelService.delete(imovel.id).subscribe({
      next: () => {
        this.loading.set(false);
        this.successMessage.set('Imóvel excluído com sucesso!');
        this.carregarImoveis();
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Erro ao deletar imóvel:', error);
        this.loading.set(false);
        this.errorMessage.set('Erro ao excluir imóvel. Tente novamente.');
      }
    });
  }

  /**
   * Deleta uma imagem específica
   */
  deletarImagem(imovelId: number, imagemId: number, index: number): void {
    if (!confirm('Tem certeza que deseja excluir esta imagem?')) {
      return;
    }

    this.imagemImovelService.delete(imagemId).subscribe({
      next: () => {
        // Remove da lista de imagens existentes
        const imagens = [...this.imagensExistentes()];
        imagens.splice(index, 1);
        this.imagensExistentes.set(imagens);

        // Recarrega os imóveis para atualizar a lista
        this.carregarImoveis();
        this.successMessage.set('Imagem excluída com sucesso!');
        setTimeout(() => this.successMessage.set(''), 3000);
      },
      error: (error) => {
        console.error('Erro ao deletar imagem:', error);
        this.errorMessage.set('Erro ao excluir imagem. Tente novamente.');
      }
    });
  }

  /**
   * Abre seletor de arquivos
   */
  abrirSeletorImagens(): void {
    this.fileInput?.nativeElement.click();
  }

  /**
   * Seleciona imagens para upload
   */
  onImageSelect(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const files = Array.from(input.files);
      const validFiles = files.filter(file => file.type.startsWith('image/'));

      if (validFiles.length !== files.length) {
        this.errorMessage.set('Por favor, selecione apenas arquivos de imagem.');
        return;
      }

      // Limita a 10 imagens por vez
      const totalImagens = this.imagensSelecionadas().length + validFiles.length;
      if (totalImagens > 10) {
        this.errorMessage.set('Você pode adicionar no máximo 10 imagens por imóvel.');
        return;
      }

      // Adiciona às imagens selecionadas
      this.imagensSelecionadas.set([...this.imagensSelecionadas(), ...validFiles]);

      // Gera previews
      validFiles.forEach(file => {
        const reader = new FileReader();
        reader.onload = (e) => {
          const preview = e.target?.result as string;
          this.imagensPreview.set([...this.imagensPreview(), preview]);
        };
        reader.readAsDataURL(file);
      });

      // Limpa o input para permitir selecionar o mesmo arquivo novamente
      input.value = '';
    }
  }

  /**
   * Remove imagem do preview (apenas novas, não salvas)
   */
  removerImagemPreview(index: number): void {
    const previews = [...this.imagensPreview()];
    previews.splice(index, 1);
    this.imagensPreview.set(previews);

    const files = [...this.imagensSelecionadas()];
    files.splice(index, 1);
    this.imagensSelecionadas.set(files);
  }

  /**
   * Obtém todas as imagens (existentes + previews)
   */
  todasImagens(): string[] {
    return [...this.imagensExistentes(), ...this.imagensPreview()];
  }

  /**
   * Verifica se pode deletar uma imagem existente
   */
  podeDeletarImagem(index: number): boolean {
    const imovel = this.editingImovel();
    return !!(imovel && imovel.imagens && imovel.imagens.length > index && imovel.imagens[index]);
  }

  /**
   * Obtém o ID da imagem no índice especificado
   */
  getImagemId(index: number): number | null {
    const imovel = this.editingImovel();
    if (imovel && imovel.imagens && imovel.imagens.length > index && imovel.imagens[index]) {
      return imovel.imagens[index].id;
    }
    return null;
  }
}
