# Introduction #

DataBinding means that you have dynamically fetched Data, which you'd like properties of your Widgets to become 'bound' towards.

# Details #

Imagine you had 3 Meta Objects in your Storage, all of type 'Prospect', and all of these objects containing these items;

  * Email
  * Name
  * State

Then if you were using an Action that somehow maps to the _Magix.MetaObjects.GetMetaObjects_ Event to fetch your Objects, such as for instance the _Magix.Demo.GetRatingObjects_ does, you would bind your Repeater with this Expression;

{DataSource[Objects](Objects.md)}

And your items inside of this Repeater like this;

{[Properties](Properties.md)[Email](Email.md)[Value](Value.md).Value}
...to bind towards the Email value of your Objects...

{[Properties](Properties.md)[Name](Name.md)[Value](Value.md).Value}
...to bind towards the Name value of your Objects...

etc...