﻿// ---------------------------------------------------------------------------
//  Copyright (c) 2018, The .NET Foundation.
//  This software is released under the Apache License, Version 2.0.
//  The license and further copyright text can be found in the file LICENSE.md
//  at the root directory of the distribution.
// ---------------------------------------------------------------------------

using System;
using System.Linq;
using Chem4Word.Model;

namespace Chem4Word.ViewModel.Commands
{
    public class DeleteCommand : BaseCommand
    {
        #region ICommand Implementation

        public override bool CanExecute(object parameter)
        {
            return MyEditViewModel.SelectedItems.Any();
        }

        public override void Execute(object parameter)
        {
            var atoms = MyEditViewModel.SelectedItems.OfType<Atom>().ToList();
            var bonds = MyEditViewModel.SelectedItems.OfType<Bond>().ToList();
            var mols = MyEditViewModel.SelectedItems.OfType<Molecule>().ToList();
            if (atoms.Any()|bonds.Any())
            {
                MyEditViewModel.UndoManager.BeginUndoBlock();
                //do any bonds remaining:  this is important if only bonds have been selected

                if (((MyEditViewModel.SelectionType & EditViewModel.SelectionTypeCode.Molecule) ==
                     EditViewModel.SelectionTypeCode.Molecule))
                {
                    foreach (Molecule mol in mols)
                    {
                        MyEditViewModel.DeleteMolecule(mol);
                    }
                }

                if (((MyEditViewModel.SelectionType & EditViewModel.SelectionTypeCode.Bond) ==
                     EditViewModel.SelectionTypeCode.Bond))
                {
                    foreach (Bond bond in bonds)
                    {
                        MyEditViewModel.DeleteBond(bond);
                    }
                }

                //do the atom and any remaining associated bonds
                if (((MyEditViewModel.SelectionType & EditViewModel.SelectionTypeCode.Atom) ==
                     EditViewModel.SelectionTypeCode.Atom))
                {
                    foreach (Atom atom in atoms)
                    {
                        MyEditViewModel.DeleteAtom(atom);
                    }
                }

               

                MyEditViewModel.UndoManager.EndUndoBlock();
            }
        }

        public override event EventHandler CanExecuteChanged;

        public DeleteCommand(EditViewModel vm) : base(vm)
        {}


        #endregion ICommand Implementation

        public override void RaiseCanExecChanged()
        {
            var args = new EventArgs();

            CanExecuteChanged?.Invoke(this, args);
        }
    }
}