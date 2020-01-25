using System;
using System.Collections.Specialized;
using InitiativeTracker.Data.Util;

namespace InitiativeTracker.Data.Stack
{
    /// <summary>
    /// This is the class that I bind to in order to display the initiative stack. An instance of it
    /// is created via XAML code and can be referenced like any other resource. 
    /// </summary>
    public class EntityStackProvider : Notifier
    {
        SortedEntityList _initiativeStack;
        int _currentTurnIndex;
        Entity _currentTurn;
        Entity[] _turnView;

        const int UPDATE_RANGE = 10;

        public static EntityStackProvider Instance;

        public SortedEntityList InitiativeStack
        {
            get { return _initiativeStack; }
            set { SetProperty(ref _initiativeStack, value); }
        }

        public Entity CurrentTurn
        {
            get { return _currentTurn; }
            set { SetProperty(ref _currentTurn, value); }
        }

        public Entity[] TurnView
        {
            get { return _turnView; }
            private set { }
        }

        public EntityStackProvider()
        {
            _initiativeStack = new SortedEntityList();
            _initiativeStack.CollectionChanged += InitiativeStack_OnCollectionChanged;
            _currentTurnIndex = 0;
            _turnView = new Entity[10];
            Instance = this;
        }

        /// <summary>
        /// Used to update turn order when entities are removed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void InitiativeStack_OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    CurrentTurn.IsHighlighted = false;
                    CurrentTurn = null;
                    _currentTurnIndex = 0;

                    for (int i = 0; i < 10; i++) _turnView[i] = null;
                    Notify("TurnView");
                    break;
                case NotifyCollectionChangedAction.Add:
                    /*
                     * currently, NewItems.Count is always equal to 1 whenever
                     * this code runs. in the future, I may implement an "AddRange"
                     * function to SortedEntityList, which will require a different algorithm.
                     */
                    if (e.NewItems.Count == 1) 
                    {
                        if (InitiativeStack.Count == 1) //first item was added
                        {
                            Entity added = (Entity)e.NewItems[0];
                            added.IsHighlighted = true;

                            if (!added.IsHidden)
                            {
                                CurrentTurn = added;
                                _turnView[0] = added;
                            }

                            _currentTurnIndex = 0;
                            Notify("TurnView");
                        }
                        else if (e.NewStartingIndex == _currentTurnIndex) //added at current turn
                        {
                            _currentTurnIndex++;
                            UpdateTurnView();
                        }
                        else if (e.NewStartingIndex < _currentTurnIndex) //added before current turn
                        {
                            _currentTurnIndex++;

                            int highestCheckIndex = _currentTurnIndex + Math.Min(UPDATE_RANGE, InitiativeStack.Count - 1);
                            if (highestCheckIndex >= InitiativeStack.Count && e.NewStartingIndex <= highestCheckIndex - InitiativeStack.Count) UpdateTurnView();
                        }
                        else if (e.NewStartingIndex <= _currentTurnIndex + UPDATE_RANGE) UpdateTurnView(); //added after current turn
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (e.OldItems.Count == 1)
                    {
                        if (InitiativeStack.Count == 0) //this was the last entity
                        {
                            CurrentTurn = null;
                            _currentTurnIndex = 0;
                            _turnView[0] = null;

                            Notify("TurnView");
                        }
                        else if (e.OldStartingIndex == _currentTurnIndex) //removed current turn
                        {
                            Entity old = (Entity)e.OldItems[0];
                            Entity current = InitiativeStack[_currentTurnIndex == InitiativeStack.Count ? 0 : _currentTurnIndex];

                            old.IsHighlighted = false;
                            current.IsHighlighted = true;

                            if (!current.IsHidden) CurrentTurn = current;
                            UpdateTurnView();
                        }
                        else if (e.OldStartingIndex < _currentTurnIndex) //removed before current turn
                        {
                            _currentTurnIndex--;

                            int highestCheckIndex = _currentTurnIndex + Math.Min(UPDATE_RANGE, InitiativeStack.Count - 1);
                            if (highestCheckIndex >= InitiativeStack.Count && e.OldStartingIndex <= highestCheckIndex - InitiativeStack.Count) UpdateTurnView();
                        }
                        else if (e.NewStartingIndex <= _currentTurnIndex + UPDATE_RANGE) UpdateTurnView(); //removed after current turn
                    }
                    break;
            }
        }

        public void Update()
        {
            UpdateTurnView();

            if (InitiativeStack[_currentTurnIndex].IsHidden && CurrentTurn != null) CurrentTurn = null;
            else if (!InitiativeStack[_currentTurnIndex].IsHidden && CurrentTurn == null) CurrentTurn = InitiativeStack[_currentTurnIndex];
        }

        void UpdateTurnView()
        {
            int i;
            int j;

            /*
             * fill up the array as far as we can initially. j is used to index the entity view
             * while i is used to index the initiative stack
             */
            for (i = _currentTurnIndex, j = 0; j < _turnView.Length && i < InitiativeStack.Count; i++, j++)
            {
                Entity entity = InitiativeStack[i];
                if (entity.IsHidden) j--; //skip hidden entities
                else _turnView[j] = entity;
            }

            //_turnView hasn't been filled up yet. try to do that now
            if (j < _turnView.Length)
            {
                for (int k = 0; k < _currentTurnIndex && j < _turnView.Length; k++, j++)
                {
                    Entity entity = InitiativeStack[k];
                    if (entity.IsHidden) j--; 
                    else _turnView[j] = entity;
                }
            }

            /*
             * if it still isn't filled up, we have empty values. update to reflect that
             * this prevents the janky feature present in older versions where entities
             * would be displayed multiple times to fill the turn view.
             */
            if (j < _turnView.Length) for (; j < _turnView.Length; j++) _turnView[j] = null;
            Notify("TurnView");
        }

        void UpdateTurn(int newIndex)
        {
            if (newIndex != _currentTurnIndex)
            {
                InitiativeStack[_currentTurnIndex].IsHighlighted = false;
                _currentTurnIndex = newIndex;

                Entity entity = InitiativeStack.WrapIndex(ref _currentTurnIndex);
                entity.IsHighlighted = true;

                if (!entity.IsHidden)
                {
                    CurrentTurn = entity;
                    UpdateTurnView();
                }
            }
        }

        public void AdvanceTurn()
        {
            if (InitiativeStack.Count > 1) //if Count == 0, there's no point in running this code
            {
                UpdateTurn(_currentTurnIndex + 1);
            }
        }

        public void RetractTurn()
        {
            if (InitiativeStack.Count > 1)
            {
                UpdateTurn(_currentTurnIndex - 1);
            }
        }

        public void SetTurn(int index)
        {
            if (InitiativeStack.Count > 1)
            {
                UpdateTurn(index);
            }
        }
    }
}