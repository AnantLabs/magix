<%@ Assembly 
    Name="Magix.Brix.Components.ActiveModules.Menu" %>

<%@ Control 
    Language="C#" 
    AutoEventWireup="true" 
    Inherits="Magix.Brix.Components.ActiveModules.Games.Tetris" %>

<link type="text/css" rel="stylesheet" href="media/modules/tetris.css"/>

<script type="text/javascript" src="media/modules/tetris.js"></script>


<div id="tetris">
	<div id="tetris-wrap">
		<div id="tetris-wrap-inner">
			<div id="tetris-main">
			</div>
			<div id="tetris-overlay">
				<div id="tetris-message"></div>
			</div>
			<div id="tetris-side">
				<div id="tetris-side-inner">
					<div id="tetris-next">
						<div id="tetris-next-inner"></div>
					</div>
					<div id="tetris-scores">
						<dl>
							<dt>Level:</dt>
							<dd id="tetris-level">1</dd>
						</dl>
						<dl>
							<dt>Score:</dt>
							<dd id="tetris-score">0</dd>
						</dl>
						<dl>
							<dt>Singles:</dt>
							<dd id="tetris-singles">0</dd>
						</dl>
						<dl>
							<dt>Doubles:</dt>
							<dd id="tetris-doubles">0</dd>
						</dl>
						<dl>
							<dt>Triples:</dt>
							<dd id="tetris-triples">0</dd>
						</dl>
						<dl>
							<dt>Tetris:</dt>
							<dd id="tetris-quads">0</dd>
						</dl>

						<div class="clear"></div>
					</div>
					<div id="tetris-keys">
						<dl>
							<dt>Move:</dt>
							<dd>&larr; &darr; &rarr;<dd>
						</dl>
						<dl>
							<dt>Rotate:</dt>
							<dd>&uarr;<dd>
						</dl>
						<dl>
							<dt>Drop:</dt>
							<dd>Spacebar<dd>
						</dl>
						<dl>
							<dt>Pauze:</dt>
							<dd>Pauze/Esc<dd>
						</dl>

						<div class="clear"></div>
					</div>
				</div>
			</div>
			<div class="clear"></div>
		</div>
	</div>
</div>
