# Languages-and-Compilers-2022-CM4106-CW

## Overview
It is the aim of the coursework to create a complete, working compiler for the given language and platform.
The language for which your compiler should work (MiniSquare22) is detailed in a separate document, which fully explains the syntax and semantics of the language. This document can be found on the module’s Moodle page. The target platform is the same Triangle Abstract Machine as was used in the labs. In other words, you must develop a compiler which turns programs written in MiniSquare22 into Triangle Abstract Language code.
You should develop your system in C# and Visual Studio. The compiler must match the overall structure and design of that taught to you in class. You may not use tools to automatically generate the parser or other parts of the compiler. You are free to use or adapt any code given to you in the lab sessions. In fact, I strongly encourage you to use the model solutions to the labs as a starting point and to adapt them for the language used for this coursework.
Your compiler will be assessed by manual inspection of the code. You should therefore adhere to software engineering best practices at all times. I would also advise that you maintain your code using a recognised Version Control System (e.g. GitHub). Use a private repository to ensure the privacy of your code

## Task(s) - format
You should submit a zip archive containing your source code and your report. You should also include any programs that you have compiled to test your work.
The section of your report describing the changes made should be no more than 3000 words. This does not include any words in tables, figures, code snippets, appendices etc. If you can cover everything in fewer words, there is no need to pad the report out. I would recommend however that you try to write around 500 words about each of the components you have implemented; if you write significantly less, you are unlikely to have described all the changes required sufficiently clearly.
There is no word or page limit for the section of the report on testing. Take as many or as few words as you need to describe the testing you conducted.
There is no need to include a cover page, tables of content etc. in the report. As shown in the feedback grids, there is no credit for report presentation, beyond it being clearly written.
Please note: As per the new university regulations, your mark will be reduced by a grade if you exceed the word limit by over 10%. This penalty applies to this whole coursework, not just the report section. (Sorry, nothing I can do about this – its official university rules.) See https://campusmoodle.rgu.ac.uk/mod/resource/view.php?id=4273573

### Sections
Your coursework will be marked in 5 sections, with each section worth an equal amount towards your overall coursework grade.

#### Scanner
Your Scanner should be developed to read in a source file written in the given source language. The characters in the source file should be compiled into a sequence of recognised tokens. The output should be a sequence of tokens corresponding to the input source code program. Your scanner should remove white space and comments. If it is not possible to tokenise the input, errors should be reported to the user in a way that helps them debug their code

#### Parser
Your Parser should work on the sequence of tokens created by the scanner. These tokens should be used to construct and output an abstract syntax tree (AST) that represents the structure of the source code program. Your Parser should be able to identify syntax errors in the source program and produce errors for sequences of instructions that do not conform to the language definition. These errors should be provided to the user in a way that helps them understand and remove the error.
You must create a new type of AST node for any new element that has been added to the abstract grammar.  For example, if a foreach command has been added, you must create a ForEachCommandNode class, rather than working out a way to achieve equivalent behaviour using existing node classes. In contrast, if a rule has been changed, you should simply adapt the existing node class, e.g. if the rule for an if command now always needs an endif, there is no need to create a new IfCommandWithEndIfNode class, rather the existing IfCommandNode class can be altered.

#### Semantic Analyser
You should implement a Semantic Analyser which operates on the tree provided by the Parser. This should perform identification (annotating each identifier with its declaration) and type checking. If any problems occur with identification (e.g. undeclared identifiers or multiple declarations of the same identifier) then this should be reported to the user in terms they will understand. Similarly, incorrect types found during type-checking must be reported to the user.
#### Code Generator
Finally, you should implement your Code Generator component. This component should take the annotated abstract syntax tree from the semantic analyser and generate the code required to perform these actions on the TAM platform used in lab practical sessions.
#### Report and Testing
In addition to your code, you should also submit a report describing how you have implemented the compiler.
This report should explain the changes you have made to the code and how these relate to the difference between the assessment language and the language used in the labs.
You should also include a section in the report explaining the testing that you have performed to check the various components. You should include all programs that you compiled to test your code, either as files or as an appendix to your report. I will not be providing any test programs – it is part of the assessment for you to be able to read what is an acceptable input in the language and to write short programs that meet this definition.
Note that your compiler is not required to perform any optimisation steps.

## How you will be graded
give your final module mark as shown in the grid below. You require an overall mark of D or more to pass this module.
In the feedback grid, the first two rows refer to minor and major issues. Minor issues are things of which are small, of low importance or rarely encountered, whereas major issues are large, significantly impact on the functioning of the compiler and affect many inputs. Generally speaking, a minor issue is one that seems to show a lack of care and attention, and a major issue is one that seems to show a lack of understanding.


![image](https://user-images.githubusercontent.com/62030463/193462905-f5f17b15-da17-4694-8deb-35014b4a4ce6.png)
