Integration into game scene
---------------------------
1)	Create a GuiRoot instance (gui root node). You can have as many GuiRoot instances as you wish.
2)	Call these methods on GuiRoot
		Draw
		Activate - turns event handling on
		Deactivate - turns event handling of
		Dispose
3)	When you need to handle mouse events, which were not handled by the gui, listen to the GuiRoot events. For example, a risen mouse down event on the GuiRoot means, that no other child
	gui node has handled it.
