import { BsModalRef } from 'ngx-bootstrap/modal';
import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  title: string;
  list: any[] = [];
  closeBtnName: string;

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

}
