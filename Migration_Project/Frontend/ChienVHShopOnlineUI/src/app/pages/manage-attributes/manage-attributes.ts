import { Component, OnInit } from '@angular/core';
import { CategoryService } from '../../services/category.service';
import { ModelService } from '../../services/model.service';
import { ColorService } from '../../services/color.service';
import { StorageService } from '../../services/storage.service';
import { CategoryModel } from '../../models/category';
import { ColorModel } from '../../models/color';
import { ProdModel } from '../../models/model';
import { StorageModel } from '../../models/storage';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

declare const bootstrap: any;

@Component({
  selector: 'app-manage-attributes',
  imports: [CommonModule, FormsModule],
  templateUrl: './manage-attributes.html',
  styleUrls: ['./manage-attributes.css']
})
export class ManageAttributesComponent implements OnInit {
  attributes: {
    key: 'category' | 'model' | 'color' | 'storage',
    title: string,
    items: { id: number; name: string }[],
  }[] = [];

  // Modal form
  formValue: string = '';
  editingTitle: string = '';
  editingKey: string = '';
  editingId: number | null = null;

  constructor(
    private categoryService: CategoryService,
    private modelService: ModelService,
    private colorService: ColorService,
    private storageService: StorageService
  ) {}

  ngOnInit(): void {
    this.loadAll();
  }

  loadAll() {
    this.categoryService.getAll().subscribe(data => this.setItems('category', 'Categories', data));
    this.modelService.getAll().subscribe(data => this.setItems('model', 'Models', data));
    this.colorService.getAll().subscribe(data => this.setItems('color', 'Colors', data));
    this.storageService.getAll().subscribe(data => this.setItems('storage', 'Storage Types', data));
  }

  setItems(key: any, title: string, data: any[]) {
    const existing = this.attributes.find(a => a.key === key);
    if (existing) {
      existing.items = data;
    } else {
      this.attributes.push({ key, title, items: data });
    }
  }

  openCreate(attr: any) {
    this.editingId = null;
    this.editingKey = attr.key;
    this.editingTitle = `Add ${attr.title.slice(0, -1)}`;
    this.formValue = '';
    this.showModal();
  }

  openEdit(attr: any, item: any) {
    this.editingId = item.id;
    this.editingKey = attr.key;
    this.editingTitle = `Edit ${attr.title.slice(0, -1)}`;
    this.formValue = item.name;
    this.showModal();
  }

  onSubmit() {
    const key = this.editingKey;
    const name = this.formValue.trim();
    if (!name) return;

    const updateList = () => this.loadAll();

    if (this.editingId == null) {
      // Create
      switch (key) {
        case 'category': this.categoryService.create(name).subscribe(updateList); break;
        case 'model': this.modelService.create(name).subscribe(updateList); break;
        case 'color': this.colorService.create(name).subscribe(updateList); break;
        case 'storage': this.storageService.create(name).subscribe(updateList); break;
      }
    } else {
      // Update
      switch (key) {
        case 'category': this.categoryService.update(this.editingId, name).subscribe(updateList); break;
        case 'model': this.modelService.update(this.editingId, name).subscribe(updateList); break;
        case 'color': this.colorService.update(this.editingId, name).subscribe(updateList); break;
        case 'storage': this.storageService.update(this.editingId, name).subscribe(updateList); break;
      }
    }

    this.hideModal();
  }

  deleteItem(attr: any, id: number) {
    if (!confirm('Are you sure you want to delete this item?')) return;

    switch (attr.key) {
      case 'category': this.categoryService.delete(id).subscribe(() => this.loadAll()); break;
      case 'model': this.modelService.delete(id).subscribe(() => this.loadAll()); break;
      case 'color': this.colorService.delete(id).subscribe(() => this.loadAll()); break;
      case 'storage': this.storageService.delete(id).subscribe(() => this.loadAll()); break;
    }
  }

  showModal() {
    const modalElement = document.getElementById('attributeModal');
    if (modalElement) {
      const modal = new bootstrap.Modal(modalElement);
      modal.show();
    }
  }

  hideModal() {
    const modalElement = document.getElementById('attributeModal');
    if (modalElement) {
      const modal = bootstrap.Modal.getInstance(modalElement);
      modal.hide();
    }
  }
}
